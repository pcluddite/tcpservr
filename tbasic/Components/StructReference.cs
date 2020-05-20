// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tbasic.Components
{
    /// <summary>
    /// Can be used to get around some struct copying
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class StructReference<T> where T : struct
    {
        private T innerStruct;

        public T Value
        {
            get {
                return innerStruct;
            }
        }

        public StructReference(T value)
        {
            innerStruct = value;
        }

        public static implicit operator StructReference<T>(T value)
        {
            return new StructReference<T>(value);
        }

        public static explicit operator T(StructReference<T> reference)
        {
            return reference.innerStruct;
        }
    }
}
