// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using Tbasic.Components;
using Tbasic.Errors;
using Tbasic.Parsing;
using Tbasic.Runtime;
using System.Collections.Generic;

namespace Tbasic.Types
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

        public IfBlock(int index, IList<Line> fullCode)
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

        public override void Execute(TRuntime runtime)
        {
            if (!Header.Text.EndsWithIgnoreCase(" then")) {
                throw ThrowHelper.ExpectedToken("THEN");
            }

            ExpressionEvaluator eval = new ExpressionEvaluator(
                Header.Text.TB_Segment(
                    Header.Text.IndexOf(' ') + 1, // Get rid of the IF
                    Header.Text.LastIndexOf(' ') - 2), // Get rid of the THEN
                runtime
            );

            if (eval.EvaluateBool()) {
                runtime.Execute(Body);
            }
            else if (HasElseBlock) {
                runtime.Execute(Else.Body);
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

            public override void Execute(TRuntime exec)
            {
                throw new NotImplementedException();
            }
        }
    }
}
