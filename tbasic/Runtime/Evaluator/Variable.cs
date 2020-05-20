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
using System.Collections.Generic;
using System.Text;
using Tbasic.Errors;
using Tbasic.Parsing;
using System;
using Tbasic.Components;

namespace Tbasic.Runtime
{
    internal class Variable : IEvaluator
    {
        private StringSegment _expression = null;

        #region Properties

        public int[] Indices { get; private set; }

        public bool IsMacro
        {
            get {
                return Name[0] == '@';
            }
        }

        public bool IsValid
        {
            get {
                return Name[Name.Length - 1] == '$';
            }
        }

        public ObjectContext CurrentContext
        {
            get {
                return CurrentExecution.Context;
            }
        }

        public Executer CurrentExecution { get; set; }
        public StringSegment Name { get; private set; }

        public StringSegment Expression
        {
            get {
                return _expression;
            }
            set {
                _expression = value.Trim();
            }
        }

        #endregion

        public Variable(StringSegment full, StringSegment name, int[] indices, Executer exec)
        {
            CurrentExecution = exec;
            _expression = full;
            Name = name;
            Indices = indices;
        }

        public override string ToString()
        {
            return _expression.ToString();
        }

        public object Evaluate()
        {
            object obj = CurrentContext.GetVariable(Name.ToString());
            if (Indices != null) {
                obj = CurrentContext.GetArrayAt(Name.ToString(), Indices);
            }
            return obj;
        }

        public string GetFullName()
        {
            return GetFullName(Name.ToString(), Indices);
        }

        public static string GetFullName(string name, int[] indices)
        {
            StringBuilder sb = new StringBuilder(name);
            if (indices != null && indices.Length > 0) {
                sb.Append("[");
                for (int i = 0; i < indices.Length; ++i) {
                    sb.AppendFormat("{0},", indices[i]);
                }
                sb.AppendFormat("{0}]", indices[0]);
            }
            return sb.ToString();
        }
    }
}