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
using Tbasic.Components;
using Tbasic.Errors;
using Tbasic.Parsing;
using Tbasic.Runtime;

namespace Tbasic
{
    internal class IfBlock : CodeBlock
    {
        public CodeBlock Else { get; private set; }

        public bool HasElseBlock
        {
            get {
                return Else != null;
            }
        }

        public override int Length
        {
            get {
                if (HasElseBlock) {
                    return base.Length + Else.Length + 1; // There's an ELSE keyword too
                }
                else {
                    return base.Length;
                }
            }
        }

        public IfBlock(int index, LineCollection fullCode)
        {
            Else = null;
            Header = fullCode[index];

            LineCollection ifLines = new LineCollection();
            LineCollection elseLines = new LineCollection();

            int expected_endif = 1; // How many 'END IF' statements are expected
            index++;

            bool isElse = false; // Whether this should be added to the else block

            for (; index < fullCode.Count; index++) {
                Line cur = fullCode[index];
                if (cur.Name.EqualsIgnoreCase("IF")) {
                    expected_endif++;
                }

                if (expected_endif > 0) {
                    if (expected_endif == 1 && cur.Name.EqualsIgnoreCase("ELSE")) { // we are now in an else block
                        isElse = true;
                        continue; // We don't need to add the word 'ELSE'
                    }
                }

                if (cur.Text.EqualsIgnoreCase("END IF")) {
                    expected_endif--;
                }

                if (expected_endif == 0) {
                    if (elseLines.Count > 0) {
                        Else = new ElseBlock(elseLines);
                    }
                    Footer = cur;
                    Body = ifLines;
                    return;
                }

                if (isElse) {
                    elseLines.Add(cur);
                }
                else {
                    ifLines.Add(cur);
                }
            }

            throw ThrowHelper.UnterminatedBlock(Header.VisibleName);
        }

        public override void Execute(Executer exec)
        {
            if (!Header.Text.EndsWithIgnoreCase(" then")) {
                throw ThrowHelper.ExpectedToken("THEN");
            }

            Evaluator eval = new Evaluator(
                new StringSegment(Header.Text,
                Header.Text.IndexOf(' ') + 1, // Get rid of the IF
                Header.Text.LastIndexOf(' ') - 2), // Get rid of the THEN
                exec
            );

            if (eval.EvaluateBool()) {
                exec.Execute(Body);
            }
            else if (HasElseBlock) {
                exec.Execute(Else.Body);
            }
        }

        private class ElseBlock : CodeBlock
        {
            internal ElseBlock(LineCollection lines)
            {
                Body = lines;
                Header = lines[0];
                Footer = lines[lines.Count - 1];
            }

            public override void Execute(Executer exec)
            {
                throw new NotImplementedException();
            }
        }
    }
}
