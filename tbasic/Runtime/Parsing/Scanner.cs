/**
 *  TBASIC
 *  Copyright (C) 2013-2016 Timothy Baxendale
 *  
 *  This library is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU Lesser General Public
 *  License as published by the Free Software Foundation; either
 *  version 2.1 of the License, or (at your option) any later version.
 *  
 *  This library is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 *  Lesser General Public License for more details.
 *  
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with this library; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301
 *  USA
 **/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tbasic.Components;
using Tbasic.Errors;
using Tbasic.Operators;
using Tbasic.Runtime;

namespace Tbasic.Parsing
{
    /// <summary>
    /// Similar idea to java.util.Scanner (don't sue me Oracle)
    /// </summary>
    internal class Scanner : Stream
    {
        private StringSegment _buffer;
        private int position;

        public override bool CanRead
        {
            get {
                return true;
            }
        }

        public override bool CanSeek
        {
            get {
                return true;
            }
        }

        public override bool CanWrite
        {
            get {
                return false;
            }
        }

        public override long Length
        {
            get {
                return _buffer.Length;
            }
        }

        public override long Position {
            get {
                return position;
            }
            set {
                position = Convert.ToInt32(value);
            }
        }

        public bool EndOfStream
        {
            get {
                return position >= _buffer.Length;
            }
        }

        public int IntPosition
        {
            get {
                return position;
            }
            set {
                position = value;
            }
        }

        public Scanner(StringSegment buffer)
        {
            _buffer = buffer;
        }

        public void SkipWhiteSpace()
        {
            if (EndOfStream)
                return;
            if (char.IsWhiteSpace(_buffer[position])) {
                do {
                    ++position;
                }
                while (char.IsWhiteSpace(_buffer[position]));
            }
        }

        public bool NextPositiveInt(out int integer)
        {
            integer = 0;
            if (EndOfStream)
                return false;
            int newPos = FindConsecutiveDigits(_buffer, position);
            if (newPos > position) {
                integer = int.Parse(_buffer.Substring(position, newPos - position));
                position = newPos;
                return true;
            }
            return false;
        }

        public bool NextPositiveNumber(out Number num)
        {
            num = default(Number);
            if (EndOfStream)
                return false;
            int endPos = FindConsecutiveDigits(_buffer, position);
            if (endPos > position) {
                if (endPos < _buffer.Length && _buffer[endPos] == '.') {
                    endPos = FindConsecutiveDigits(_buffer, endPos + 1);
                }
                if (endPos < _buffer.Length && (_buffer[endPos] == 'e' || _buffer[endPos] == 'E')) {
                    if (_buffer[++endPos] == '-')
                        ++endPos;
                    endPos = FindConsecutiveDigits(_buffer, endPos);
                }
            }
            else {
                return false;
            }
            num = Number.Parse(_buffer.Substring(position, endPos - position));
            position = endPos;
            return true;
        }

        public bool NextHexadecimal(out int number)
        {
            number = 0;
            if (EndOfStream)
                return false;

            int endPos = position;
            if (_buffer[endPos++] != '0')
                return false;
            if (endPos >= _buffer.Length || _buffer[endPos++] != 'x')
                return false;
            endPos = FindConsecutiveHex(_buffer, endPos);
            number = Convert.ToInt32(_buffer.Substring(position, endPos - position));
            position = endPos;
            return true;
        }

        private static unsafe int FindConsecutiveDigits(StringSegment expr, int start)
        {
            fixed (char* lpfullstr = expr.FullString)
            {
                char* lpseg = lpfullstr + expr.Offset;
                int len = expr.Length;
                int index = start;
                for (; index < len; ++index) {
                    if (!char.IsDigit(lpseg[index])) {
                        return index;
                    }
                }
                return index;
            }
        }

        private static unsafe int FindConsecutiveHex(StringSegment expr, int start)
        {
            fixed (char* lpfullstr = expr.FullString)
            {
                char* lpseg = lpfullstr + expr.Offset;
                int len = expr.Length;
                int index = start;
                for (; index < len; ++index) {
                    if (!IsHexDigit(lpseg[index])) {
                        return index;
                    }
                }
                return index;
            }
        }

        private static bool IsHexDigit(char c)
        {
            return char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        }

        public bool Next(string pattern, bool ignoreCase = true)
        {
            if (EndOfStream)
                return false;
            if (!_buffer.Subsegment(position).StartsWith(pattern, ignoreCase))
                return false;
            position += pattern.Length;
            return true;
        }

        public bool NextString(out string parsed)
        {
            if (EndOfStream || (_buffer[position] != '\"' && _buffer[position] != '\'')) {
                parsed = null;
                return false;
            }
            int endPos = GroupParser.ReadString(_buffer, position, out parsed) + 1;
            position = endPos;
            return true;
        }

        public bool NextFunction(Executer exec, out Function func)
        {
            func = null;
            if (EndOfStream)
                return false;
            int originalPos = position;
            if (char.IsLetter(_buffer[position]) || _buffer[position] == '_') {
                position = FindAcceptableFuncChars(_buffer, ++position);
                if (position < _buffer.Length) {
                    StringSegment name = _buffer.Subsegment(originalPos, position - originalPos);
                    SkipWhiteSpace();
                    if (Next("(")) {
                        IList<object> args;
                        position = GroupParser.ReadGroup(_buffer, position - 1, exec, out args) + 1;
                        func = new Function(exec);
                        func.Parse(_buffer.Subsegment(originalPos, position - originalPos), name, args);
                        return true;
                    }
                }
            }
            position = originalPos;
            return false;
        }

        public bool NextVariable(Executer exec, out Variable variable)
        {
            variable = null;
            if (EndOfStream)
                return false;
            int originalPos = position;
            if (char.IsLetter(_buffer[position]) || _buffer[position] == '_') {
                position = FindAcceptableFuncChars(_buffer, ++position);
                if (!EndOfStream && _buffer[position++] == '$') {
                    StringSegment name = _buffer.Subsegment(originalPos, position - originalPos);
                    SkipWhiteSpace();
                    int[] indices;
                    if (!NextIndices(exec, out indices))
                        indices = null;
                    variable = new Variable(_buffer, name, indices, exec);
                    return true;
                }
            }
            else if (_buffer[position] == '@') { // it's a macro
                return NextMacro(exec, out variable);
            }
            position = originalPos;
            return false;
        }

        /// <summary>
        /// This assumes that the first char has already been checked to be an '@'!
        /// </summary>
        /// <param name="exec"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        private bool NextMacro(Executer exec, out Variable variable)
        {
            int originalPos = position;
            if (++position < _buffer.Length) {
                position = FindAcceptableFuncChars(_buffer, position);
                StringSegment name = _buffer.Subsegment(originalPos, position - originalPos);
                SkipWhiteSpace();
                int[] indices;
                if (!NextIndices(exec, out indices))
                    indices = null;
                variable = new Variable(_buffer, name, indices, exec);
                return true;
            }
            variable = null;
            position = originalPos;
            return false;
        }

        public bool NextIndices(Executer exec, out int[] indices)
        {
            indices = null;
            int originalPos = position;
            if (!EndOfStream && _buffer[position] == '[') {
                IList<object> args;
                position = GroupParser.ReadGroup(_buffer, position, exec, out args) + 1;
                indices = new int[args.Count];
                for (int i = 0; i < args.Count; ++i) {
                    int? index = args[i] as int?;
                    if (index == null) {
                        throw ThrowHelper.InvalidTypeInExpression(args[i].GetType().Name, typeof(int).Name);
                    }
                    else {
                        indices[i] = index.Value;
                    }
                }
                return true;
            }
            position = originalPos;
            return false;
        }
        
        private static unsafe int FindAcceptableFuncChars(StringSegment expr, int start)
        {
            fixed (char* lpfullstr = expr.FullString)
            {
                char* lpseg = lpfullstr + expr.Offset;
                int len = expr.Length;
                int index = start;
                for (; index < len; ++index) {
                    if (!char.IsLetterOrDigit(lpseg[index]) && lpseg[index] != '_') {
                        return index;
                    }
                }
                return index;
            }
        }

        public bool NextBool(out bool b)
        {
            if (EndOfStream) {
                b = false;
                return false;
            }
            int currPos = position;
            if (Next(bool.TrueString)) {
                b = true;
                position += bool.TrueString.Length;
                return true;
            }
            position = currPos; // reset the the pos when method was called
            b = !Next(bool.FalseString);
            if (!b) {
                position += bool.FalseString.Length;
                return true;
            }
            position = currPos;
            return false;
        }

        public bool NextBinaryOp(BinOpDictionary _binOps, out BinaryOperator foundOp)
        {
            if (EndOfStream || !MatchOperator(_buffer, position, _binOps, out foundOp)) {
                foundOp = default(BinaryOperator);
                return false;
            }
            position += foundOp.OperatorString.Length;
            return true;
        }

        public bool NextUnaryOp(UnaryOpDictionary _unOps, object last, out UnaryOperator foundOp)
        {
            if (EndOfStream || (last != null && !(last is BinaryOperator))) {
                foundOp = default(UnaryOperator);
                return false;
            }
            if (!MatchOperator(_buffer, position, _unOps, out foundOp))
                return false;
            position += foundOp.OperatorString.Length;
            return true;
        }

        private static bool MatchOperator<T>(StringSegment expr, int index, IDictionary<string, T> ops, out T foundOp) where T : IOperator
        {
            string foundStr = null;
            foundOp = default(T);
            foreach (var op in ops) {
                string opStr = op.Value.OperatorString;
                if (expr.StartsWith(opStr, index, ignoreCase: true)) {
                    foundOp = op.Value;
                    foundStr = opStr;
                }
            }
            return foundStr != null;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin) {
                case SeekOrigin.Begin:
                    Position = offset;
                    return Position;
                case SeekOrigin.Current:
                    Position += offset;
                    return Position;
                case SeekOrigin.End:
                    Position = Length - 1 + offset;
                    return Position;
            }
            return 0;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override void Flush()
        {
            // no-op
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (Position + offset >= Length)
                return 0;
            return Encoding.Unicode.GetBytes(_buffer.ToString(), Convert.ToInt32(Position), count, buffer, offset);
        }
    }
}
