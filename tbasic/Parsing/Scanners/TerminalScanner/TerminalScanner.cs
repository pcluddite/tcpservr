// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System.Collections.Generic;

namespace Tbasic.Parsing
{
    internal class TerminalScanner : DefaultScanner
    {
        public TerminalScanner(string buffer)
            : base(buffer)
        {
        }

        protected override char EscapeCharacter
        {
            get {
                return '`'; // use the backtick as an escape character. this way we don't have to constantly use \\ in paths.
            }
        }

        public override IScanner Scan(IEnumerable<char> buffer)
        {
            return new TerminalScanner(buffer.ToString());
        }

        public override bool NextFunction(out IEnumerable<char> name, out IList<IEnumerable<char>> args)
        {
            int start = Position;
            if (base.NextFunction(out name, out args)) {
                return true;
            }
            else if (NextValidIdentifier(out name)) {
                args = new List<IEnumerable<char>>();
                IEnumerable<char> token;
                while (NextWordOrString(out token)) {
                    args.Add(token);
                }
                return true;
            }
            else {
                Position = start;
                return false;
            }
        }
    }
}
