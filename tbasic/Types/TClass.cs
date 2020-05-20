// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using Tbasic.Parsing;
using Tbasic.Runtime;

namespace Tbasic.Types
{
    /// <summary>
    /// Represents a Tbasic class
    /// </summary>
    public class TClass : ObjectContext, ICloneable
    {
        /// <summary>
        /// The name of this class
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// The parent class
        /// </summary>
        public TClass ParentClass { get; private set; } = null;
        /// <summary>
        /// The lines in the constructor. This includes any field initializations.
        /// </summary>
        public LineCollection Constructor { get; private set; }
        
        private TClass()
        {
        }

        /// <summary>
        /// Generate a base class
        /// </summary>
        public TClass(string name, ObjectContext global) : base(global)
        {
            Name = name;
            Constructor = new LineCollection();
        }

        /// <summary>
        /// Generate a child class
        /// </summary>
        public TClass(string name, TClass parent) : base(parent.ParentContext)
        {
            Name = name;
            ParentClass = parent;
            Constructor = new LineCollection();
        }

        /// <summary>
        /// Checks if this class inherits from a parent class
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool Inherits(TClass parent)
        {
            TClass myancestor = ParentClass;
            while (myancestor != null) {
                if (myancestor == parent)
                    return true;
                myancestor = myancestor.ParentClass;
            }
            return false;
        }

        private Stack<TClass> GetLineage()
        {
            Stack<TClass> lineage = new Stack<TClass>();
            TClass curr = this;
            do {
                lineage.Push(curr);
                curr = curr.ParentClass;
            }
            while (curr != null);
            return lineage;
        }

        /// <summary>
        /// Creates an instance of this class for use in Tbasic code
        /// </summary>
        public TClass GetInstance(TRuntime runtime, StackData stackdat)
        {
            TClass instance = Clone();
            instance.SetVariable("@this", instance);
            instance.SetVariable("@base", instance.ParentClass);

            Stack<TClass> lineage = GetLineage();
            do {
                lineage.Pop().Ctor(instance, runtime, stackdat); // construct all ancestors
            }
            while (lineage.Count > 0);
            return instance;
        }

        private void Ctor(TClass instance, TRuntime runtime, StackData stackdat)
        {
            ObjectContext old = runtime.Context;
            runtime.Context = instance;
            runtime.Execute(Constructor); // this is to initialize all the variables

            CallData ctor;
            if (TryGetFunction("<>ctor", out ctor)) {
                ctor.Function(runtime, stackdat); // do the user defined constructor
            }

            runtime.Context = old;
        }

        internal TClass Clone()
        {
            TClass cloned = new TClass();
            CopyTo(this, cloned);
            cloned.Name = Name;
            cloned.ParentClass = ParentClass?.Clone();
            cloned.Constructor = Constructor;
            return cloned;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
