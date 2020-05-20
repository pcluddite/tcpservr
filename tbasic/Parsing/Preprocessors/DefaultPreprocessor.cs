// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Tbasic.Errors;
using Tbasic.Runtime;
using Tbasic.Types;

namespace Tbasic.Parsing
{
    internal class DefaultPreprocessor : IPreprocessor
    {
        public const char CommentChar = '#';
        public const char ContinueChar = '_';
        public const char VarSigil = '$';

        protected virtual Predicate<Line> ClassBegin { get; } = (o => o.Name.EqualsIgnoreCase("CLASS"));
        protected virtual Predicate<Line> ClassEnd { get; } = (o => o.Text.EqualsIgnoreCase("END CLASS"));
        protected virtual Predicate<Line> FuncBegin { get; } = (line => line.Name.EqualsIgnoreCase("FUNCTION"));
        protected virtual Predicate<Line> FuncEnd { get; } = (line => line.Text.EqualsIgnoreCase("END FUNCTION"));

        public ICollection<FunctionBlock> Functions { get; } = new List<FunctionBlock>();
        public ICollection<TClass> Classes { get; } = new List<TClass>();
        public LineCollection Lines { get; } = new LineCollection();

        private TRuntime runtime;

        public DefaultPreprocessor()
        {
        }

        public virtual IPreprocessor Preprocess(TRuntime runtime, TextReader reader)
        {
            Functions.Clear();
            Classes.Clear();
            Lines.Clear();
            this.runtime = runtime;
            ScanLines(reader);
            return this;
        }

        protected virtual void ScanLines(TextReader reader)
        {
            int lineNumber = 1;
            Line line;
            while (ProcessCodeLine(reader, lineNumber++, out line)) {
                if (string.IsNullOrEmpty(line.Text) || line.Text[0] == CommentChar)
                    continue;

                if (FuncBegin(line)) {
                    FunctionBlock func;
                    lineNumber = ProcessFuncBlock(reader, line, out func);
                    Functions.Add(func);
                }
                else if (ClassBegin(line)) {
                    TClass tclass;
                    lineNumber = ProcessClassBlock(reader, line, out tclass);
                    Classes.Add(tclass);
                }
                else {
                    Lines.Add(line);
                }
            }
        }

        protected virtual bool ProcessCodeLine(TextReader reader, int lineNumber, out Line line)
        {
            string linestr = reader.ReadLine()?.Trim();
            if (linestr == null) {
                line = default(Line);
                return false;
            }
            if (linestr.Length > 0 && linestr[linestr.Length - 1] == ContinueChar) {
                Stack<char> chars = new Stack<char>(linestr);
                do {
                    chars.Pop(); // remove last '_' character
                    linestr = reader.ReadLine()?.Trim();
                    if (linestr == null)
                        throw new EndOfCodeException($"Line continuation character '{ContinueChar}' cannot end script");
                    foreach (char c in linestr)
                        chars.Push(c);
                }
                while (chars.Peek() == ContinueChar);
                linestr = new string(chars.Reverse().ToArray());
            }
            line = Line.CreateLineNoTrim(lineNumber, linestr);
            return true;
        }

        protected virtual int ProcessFuncBlock(TextReader reader, Line current, out FunctionBlock block)
        {
            Line header = current;
            LineCollection body = new LineCollection();
            int lineNumber = current.LineNumber + 1;
            while (ProcessCodeLine(reader, lineNumber++, out current) && !FuncEnd(current)) {
                body.Add(current);
            }
            if (current.Text == null)
                throw ThrowHelper.UnterminatedBlock("function");
            block = new FunctionBlock(GetPrototype(header), header, current, body);
            return lineNumber;
        }

        protected virtual IList<string> GetPrototype(Line header)
        {
            IScanner scanner = runtime.Scanner.Scan(header.Text);
            scanner.Next("FUNCTION");
            scanner.SkipWhiteSpace();
            IEnumerable<char> funcname;
            if (!scanner.NextValidIdentifier(out funcname))
                throw new InvalidDefinitionException("Name contains invalid characters or was not present", "function");
            IList<IEnumerable<char>> args;
            scanner.NextGroup(out args);
            string[] ret = new string[args.Count + 1];
            ret[0] = funcname.ToString();
            for (int i = 1; i < ret.Length; ++i) {
                ret[i] = args[i - 1].ToString();
            }
            return ret;
        }

        protected virtual int ProcessClassBlock(TextReader reader, Line current, out TClass tclass)
        {
            int nline = current.LineNumber + 1;
            IScanner scanner = runtime.Scanner.Scan(current.Text);
            scanner.Next("CLASS");
            scanner.SkipWhiteSpace();
            IEnumerable<char> classname;
            if (!scanner.NextValidIdentifier(out classname))
                throw new InvalidDefinitionException("Name contains invalid characters or was not present", "class");

            tclass = new TClass(classname.ToString(), runtime.Global);

            while (ProcessCodeLine(reader, nline++, out current) && !ClassEnd(current)) {
                if (string.IsNullOrEmpty(current.Text) || current.Text[0] == CommentChar)
                    continue;

                if (current.Name.EqualsIgnoreCase("DIM")) {
                    tclass.Constructor.Add(current);
                }
                else if (FuncBegin(current)) {
                    FunctionBlock func;
                    nline = ProcessFuncBlock(reader, current, out func);
                    tclass.AddFunction(func.Prototype[0], func.Execute);
                }
                else if (current.Name.EqualsIgnoreCase(tclass.Name)) {
                    current.Text = "FUNCTION " + current.Text; // this is just to satisfy the parser. Try to fix later. 8/26/16
                    FunctionBlock ctor;
                    nline = ProcessFuncBlock(reader, current, out ctor);
                    tclass.AddFunction("<>ctor", ctor.Execute);
                }
                else {
                    throw new InvalidTokenException(current.Text);
                }
            }
            if (current.Text == null)
                throw ThrowHelper.UnterminatedBlock("class");
            
            return nline;
        }
    }
}
