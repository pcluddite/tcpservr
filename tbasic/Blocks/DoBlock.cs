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
using Tbasic.Runtime;
using Tbasic.Errors;
using Tbasic.Parsing;
using Tbasic.Components;

namespace Tbasic
{
    internal class DoBlock : CodeBlock
    {
        public DoBlock(int index, LineCollection code)
        {
            LoadFromCollection(
                code.ParseBlock(
                    index,
                    c => c.Name.EqualsIgnoreCase("DO"),
                    c => c.Text.EqualsIgnoreCase("LOOP")
                ));
        }

        public override void Execute(Executer exec)
        {
            TFunctionData parameters = new TFunctionData(exec, Header.Text);

            if (parameters.ParameterCount < 3) {
                throw ThrowHelper.NoCondition();
            }

            StringSegment condition = new StringSegment(Header.Text, Header.Text.IndexOf(' ', 3));

            if (parameters.GetParameter<string>(1).EqualsIgnoreCase("UNTIL")) {
                condition = new StringSegment(string.Format("NOT ({0})", condition)); // Until means inverted
            }
            else if (parameters.GetParameter<string>(1).EqualsIgnoreCase("WHILE")) {
                // don't do anything, you're golden
            }
            else {
                throw ThrowHelper.ExpectedToken("UNTIL' or 'WHILE");
            }

            Evaluator eval = new Evaluator(condition, exec);

            do {
                exec.Execute(Body);
                if (exec.BreakRequest) {
                    exec.HonorBreak();
                    break;
                }
                eval.ShouldParse = true;
            }
            while (eval.EvaluateBool());
        }
    }
}
