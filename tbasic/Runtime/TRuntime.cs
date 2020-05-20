// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using Tbasic.Errors;
using Tbasic.Parsing;
using Tbasic.Types;

namespace Tbasic.Runtime
{
    /// <summary>
    /// An event handler for a user exit
    /// </summary>
    /// <param name="sender">the object that has sent this message</param>
    /// <param name="e">any additional event arguments</param>
    public delegate void UserExittedEventHandler(object sender, EventArgs e);

    /// <summary>
    /// Executes a script and stores information on the current runtime
    /// </summary>
    public class TRuntime
    {
        /// <summary>
        /// A string containing information on this version of Tbasic
        /// </summary>
        public const string VERSION = "TBASIC BETA 0.4.0.2016";

        #region Properties
        /// <summary>
        /// Gets or sets the scanner
        /// </summary>
        public IScanner Scanner { get; set; } = Scanners.Default;
        /// <summary>
        /// Gets or sets the preprocessor
        /// </summary>
        public IPreprocessor Preprocessor { get; set; } = Preprocessors.Default;

        /// <summary>
        /// The global context for this object
        /// </summary>
        public ObjectContext Global { get; private set; }

        /// <summary>
        /// Gets or sets the rules that the runtime should adhere to
        /// </summary>
        public ExecuterOption Options { get; set; } = ExecuterOption.None;

        /// <summary>
        /// Gets if request to break has been petitioned
        /// </summary>
        public bool BreakRequest { get; internal set; }

        /// <summary>
        /// Gets the current line in the script that the executer is processing
        /// </summary>
        public int CurrentLine { get; internal set; }

        /// <summary>
        /// Gets or sets the ObjectContext in which the code is executed
        /// </summary>
        public ObjectContext Context { get; set; }

        /// <summary>
        /// Gets if a request to exit has been petitioned. This should apply to the scope of the whole application.
        /// </summary>
        public static bool ExitRequest { get; internal set; }

        /// <summary>
        /// Raised when a user has requested to exit
        /// </summary>
        public event UserExittedEventHandler OnUserExitRequest;

        #endregion

        /// <summary>
        /// Initializes a new object to execute scripts or single lines of code
        /// </summary>
        public TRuntime()
        {
            Global = new ObjectContext(null);
            Context = Global;
            CurrentLine = 0;
            BreakRequest = false;
        }

        /// <summary>
        /// Runs a script
        /// </summary>
        /// <param name="script">the full text of the script to process</param>
        public void Execute(string script)
        {
            Contract.Requires(script != null);
            Execute(new StringReader(script));
        }

        /// <summary>
        /// Runs a script
        /// </summary>
        /// <param name="reader">the lines of the script to process</param>
        public void Execute(TextReader reader)
        {
            Contract.Requires(reader != null);
            IPreprocessor p = Preprocessor.Preprocess(this, reader);
            if (p.Functions.Count > 0) {
                foreach (FunctionBlock func in p.Functions) {
                    Global.AddFunction(func.Prototype[0], func.Execute);
                }
            }
            if (p.Classes.Count > 0) {
                foreach(TClass t in p.Classes) {
                    Global.AddClass(t.Name, t);
                }
            }
            Execute(p.Lines);

            /*if (ManagedWindows.Count != 0 && !this.ExitRequest) {
                System.Windows.Forms.Application.Run(new FormLoader(this));
            }*/
        }

        /// <summary>
        /// Executes a single line of code
        /// </summary>
        /// <param name="codeLine"></param>
        /// <returns></returns>
        public StackData Execute(Line codeLine)
        {
#if !NO_CATCH
            TbasicRuntimeException runEx;
            try {
#endif
                return Execute(this, codeLine);
#if !NO_CATCH
            }
            catch(Exception ex) when ((runEx = TbasicRuntimeException.WrapException(ex)) != null) {
                throw runEx;
            }
#endif
        }

        internal StackData Execute(IList<Line> lines)
        {
            StackData runtime = null;
            for (int index = 0; index < lines.Count; index++) {
                if (BreakRequest) {
                    break;
                }
                Line current = lines[index];
                CurrentLine = current.LineNumber;
#if !NO_CATCH
                TbasicRuntimeException runEx;
                try {
#endif
                    BlockCreator block_init;
                    if (Context.TryGetBlock(current.Name, out block_init)) {
                        CodeBlock block = block_init(index, lines);
                        Context = Context.CreateSubContext();
                        block.Execute(this);
                        Context = Context.Collect();
                        index += block.Length - 1; // skip the length of the executed block
                    }
                    else {
                        runtime = Execute(this, current);
                    }
#if !NO_CATCH
                }
                catch (Exception ex) when ((runEx = TbasicRuntimeException.WrapException(ex)) != null) { // only catch errors that we understand 8/16/16
                    HandleError(current, runtime ?? new StackData(Options), runEx);
                }
#endif
            }
            return runtime ?? new StackData(Options);
        }

        internal static StackData Execute(TRuntime runtime, Line codeLine)
        {
            StackData stackdat;
            CallData calldat;
            if (!codeLine.IsFunction && runtime.Context.TryGetCommand(codeLine.Name, out calldat)) {
                stackdat = new StackData(runtime, codeLine.Text);
                if (calldat.ShouldEvaluate) {
                    stackdat.EvaluateAll(runtime);
                }
                stackdat.ReturnValue = calldat.Function(runtime, stackdat);
            }
            else {
                stackdat = new StackData(runtime.Options);
                ExpressionEvaluator eval = new ExpressionEvaluator(codeLine.Text, runtime);
                runtime.Context.PersistReturns(stackdat);
                stackdat.ReturnValue = eval.Evaluate();
            }
            runtime.Context.SetReturns(stackdat);
            return stackdat;
        }

        internal object ExecuteInContext(ObjectContext newcontext, TbasicFunction func, StackData stackdat)
        {
            ObjectContext old = Context;
            Context = newcontext.CreateSubContext();
            stackdat.ReturnValue = func(this, stackdat);
            Context.SetReturns(stackdat);
            Context = old;
            return stackdat.ReturnValue;
        }

        internal StackData ExecuteInContext(ObjectContext context, IList<Line> lines)
        {
            ObjectContext old = Context;
            Context = context;
            StackData dat = Execute(lines);
            Context = old;
            return dat;
        }

        internal StackData ExecuteInSubContext(IList<Line> lines)
        {
            StackData ret;
            Context = Context.CreateSubContext();
            ret = Execute(lines);
            Context = Context.Collect();
            return ret;
        }

        /// <summary>
        /// Executes a function or command with the given arguments
        /// </summary>
        public object ExecuteFunction(string name, params object[] args)
        {
            CallData calldat;
            StackData stackdat = new StackData(Options, args);
            stackdat.Name = name;

            if (Context.TryGetFunction(name, out calldat) || Context.TryGetCommand(name, out calldat)) {
                if (calldat.ShouldEvaluate) {
                    stackdat.EvaluateAll(this);
                }
                return calldat.Function(this, stackdat);
            }
            else {
                throw ThrowHelper.UndefinedFunction(name);
            }
        }

        private void HandleError(Line current, StackData stackdat, TbasicRuntimeException ex)
        {
            FunctionException cEx = ex as FunctionException;
            if (cEx != null) {
                int status = cEx.Status;
                string msg = stackdat.ReturnValue as string;
                if (string.IsNullOrEmpty(msg)) {
                    msg = cEx.Message;
                }
                stackdat.Status = status;
                stackdat.ReturnValue = msg;
                Context.SetReturns(stackdat);
                if (Options.HasFlag(ExecuterOption.ThrowErrors)) { // throw errors if the user wants it
                    throw new LineException(current.LineNumber, current.VisibleName, cEx);
                }
            }
            else {
                throw new LineException(current.LineNumber, current.VisibleName, ex);
            }
        }

        /// <summary>
        /// Requests a break from a code block
        /// </summary>
        public void RequestBreak()
        {
            BreakRequest = true;
        }

        /// <summary>
        /// Signals that a break request will be honored in the immediate future. This should only be called if you actually intend to break.
        /// </summary>
        public void HonorBreak()
        {
            if (!ExitRequest) {
                BreakRequest = false;
            }
        }

        /// <summary>
        /// Requests the script to exit. This implies a break request.
        /// </summary>
        public void RequestExit()
        {
            ExitRequest = true;
            BreakRequest = true;
            OnUserExit(EventArgs.Empty);
        }

        /// <summary>
        /// Enables an executer option
        /// </summary>
        /// <param name="option"></param>
        public void EnableOption(ExecuterOption option)
        {
            Options |= option;
        }

        /// <summary>
        /// Disables an executer option
        /// </summary>
        /// <param name="option"></param>
        public void DisableOption(ExecuterOption option)
        {
            Options &= ~option;
        }

        /// <summary>
        /// Gets whether or not an option has been enabled
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public bool IsEnforced(ExecuterOption option)
        {
            return Options.HasFlag(option);
        }

        private void OnUserExit(EventArgs e)
        {
            OnUserExitRequest?.Invoke(this, e);
        }

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(Preprocessor != null);
            Contract.Invariant(Scanner != null);
            Contract.Invariant(Context != null);
        }
    }
}
