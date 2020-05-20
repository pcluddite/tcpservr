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
using Tbasic.Operators;

namespace Tbasic.Runtime
{
    internal class BinaryOpQueue
    {
        private LinkedList<BinOpNodePair> _oplist = new LinkedList<BinOpNodePair>();

        public BinaryOpQueue(LinkedList<object> expressionlist)
        {
            LinkedListNode<object> i = expressionlist.First;
            while (i != null) {
                Enqueue(new BinOpNodePair(i));
                i = i.Next;
            }
        }

        public bool Enqueue(BinOpNodePair nodePair)
        {
            if (!nodePair.IsValid())
                return false;

            for (var currentNode = _oplist.First; currentNode != null; currentNode = currentNode.Next) {
                if (currentNode.Value.Operator.Precedence > nodePair.Operator.Precedence) {
                    _oplist.AddBefore(currentNode, nodePair);
                    return true;
                }
            }
            _oplist.AddLast(nodePair);
            return true;
        }

        public bool Dequeue(out BinOpNodePair nodePair)
        {
            if (_oplist.Count == 0) {
                nodePair = default(BinOpNodePair);
                return false;
            }
            nodePair = _oplist.First.Value;
            _oplist.RemoveFirst();
            return true;
        }

        public int Count
        {
            get { return _oplist.Count; }
        }
    }
}
