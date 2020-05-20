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

namespace Tbasic.Operators
{
    internal struct BinOpNodePair
    {
        private LinkedListNode<object> node;
        private BinaryOperator op;

        public BinaryOperator Operator
        {
            get {
                return op;
            }
        }

        public LinkedListNode<object> Node
        {
            get {
                return node;
            }
        }

        public BinOpNodePair(LinkedListNode<object> node)
        {
            this.node = node;
            var binop = node.Value as BinaryOperator?;
            if (binop == null) {
                op = default(BinaryOperator);
            }
            else {
                op = binop.Value;
            }
        }

        public bool IsValid()
        {
            return op != default(BinaryOperator);
        }
    }
}
