// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using Tbasic.Errors;
using Tbasic.Parsing;
using Tbasic.Runtime;

namespace Tbasic.Types
{
    internal class DoBlock : CodeBlock
    {
        public DoBlock(int index, IList<Line> code)
        {
            LoadFromCollection(
                LineCollection.ParseBlock(index, code,
                    c => c.Name.EqualsIgnoreCase("DO"),
                    c => c.Name.EqualsIgnoreCase("LOOP")
                ));
        }

        public override void Execute(TRuntime runtime)
        {
            IScanner scanner;
            bool doLoop;
            Statement line = new Statement(runtime.Scanner.Scan(Footer.Text));

            if (line.Count < 3) {
                line = new Statement(runtime.Scanner.Scan(Header.Text));
                if (line.Count < 3) { // still less than three? there's no condition
                    throw ThrowHelper.NoCondition();
                }
                if (Footer.Name != Footer.Text) {
                    throw new InvalidTokenException("Unexpected arguments in loop footer", prependGeneric: false);
                }
                scanner = runtime.Scanner.Scan(Header.Text);
                scanner.Skip(Header.Text.IndexOf(' ', Header.Name.Length) + 1);
                doLoop = false;
            }
            else {
                if (Header.Name != Header.Text) {
                    throw new InvalidTokenException("Unexpected arguments in loop header", prependGeneric: false);
                }
                scanner = runtime.Scanner.Scan(Footer.Text);
                scanner.Skip(Footer.Text.IndexOf(' ', Footer.Name.Length) + 1);
                doLoop = true;
            }

            Predicate<ExpressionEvaluator> condition;
            string loop = scanner.Next().ToString();

            if (string.Equals(loop, "UNTIL", StringComparison.OrdinalIgnoreCase)) {
                condition = (e => !e.EvaluateBool());
            }
            else if (string.Equals(loop, "WHILE", StringComparison.OrdinalIgnoreCase)) {
                condition = (e => e.EvaluateBool());
            }
            else {
                throw ThrowHelper.ExpectedToken("UNTIL' or 'WHILE");
            }

            ExpressionEvaluator eval = new ExpressionEvaluator(scanner.Read(scanner.Position, scanner.Length - scanner.Position), runtime);

            if (doLoop) {
                do {
                    runtime.Execute(Body);
                    if (runtime.BreakRequest) {
                        runtime.HonorBreak();
                        break;
                    }
                }
                while (condition(eval));
            }
            else {
                while (condition(eval)) {
                    runtime.Execute(Body);
                    if (runtime.BreakRequest) {
                        runtime.HonorBreak();
                        break;
                    }
                }
            }
        }
    }
}
