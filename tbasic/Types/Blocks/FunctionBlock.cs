// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.Linq;
using Tbasic.Parsing;
using Tbasic.Runtime;

namespace Tbasic.Types
{
    /// <summary>
    /// A code block that sores information on a function
    /// </summary>
    public class FunctionBlock : CodeBlock
    {
        /// <summary>
        /// Gets or sets this function's prototype (the name followed by a collection of expected arguments)
        /// </summary>
        public virtual IList<string> Prototype { get; set; }

        /// <summary>
        /// Constructs a function block without initializing any members
        /// </summary>
        protected FunctionBlock()
        {
        }

        /// <summary>
        /// Constructs a function block
        /// </summary>
        /// <param name="prototype">the name and list of arguments for the function</param>
        /// <param name="body">the lines for the body of the function</param>
        /// <param name="header">the function header</param>
        /// <param name="footer">the function footer</param>
        public FunctionBlock(IList<string> prototype, Line header, Line footer, IList<Line> body)
        {
            Header = header;
            Footer = footer;
            Body = body;
            Prototype = prototype;
        }

        /// <summary>
        /// Executes the body of the function
        /// </summary>
        public virtual object Execute(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(Prototype.Count);

            ObjectContext context = runtime.Context.CreateSubContext();

            int index = 0;
            foreach(string param in Prototype.Skip(1)) { // skip the first item in the collection, which is assumed to be the name
                context.AddVariable(param, stackdat.Get(++index));
            }
            
            context.AddCommand("return", new CallData(Return, evaluate: false));
            context.AddCommand("raise", SetStatus);

            StackData ret = runtime.ExecuteInContext(context, Body);
            stackdat.CopyFrom(ret);
            runtime.HonorBreak();
            return stackdat.ReturnValue;
        }

        private object Return(TRuntime runtime, StackData stackdat)
        {
            if (stackdat.ParameterCount < 2) {
                stackdat.AssertCount(2);
            }
            ExpressionEvaluator e = new ExpressionEvaluator(
                stackdat.Text.Substring(stackdat.Name.Length),
                runtime);
            runtime.RequestBreak();
            return stackdat.ReturnValue = e.Evaluate();
        }

        private object SetStatus(TRuntime runtime, StackData stackdat)
        {
            stackdat.AssertCount(2);
            return stackdat.Status = stackdat.Get<int>(1);
        }

        /// <summary>
        /// Do not call this method. This is not implemented by default.
        /// </summary>
        public override void Execute(TRuntime runtime)
        {
            throw new NotImplementedException();
        }
    }
}
