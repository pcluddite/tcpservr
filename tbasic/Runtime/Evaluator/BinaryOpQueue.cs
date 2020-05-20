// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System.Collections.Generic;
using Tbasic.Types;
using Tbasic.Components;
using Tbasic.Parsing;

namespace Tbasic.Runtime
{
    internal class BinaryOpQueue
    {
        private LinkedList<ValueTuple<BinaryOperator, LinkedListNode<object>>> _oplist = new LinkedList<ValueTuple<BinaryOperator, LinkedListNode<object>>>();

        public BinaryOpQueue(LinkedList<object> expressionlist)
        {
            LinkedListNode<object> obj = expressionlist.First;
            while (obj != null) {
                Enqueue(obj);
                obj = obj.Next;
            }
        }

        public bool Enqueue(LinkedListNode<object> node)
        {
            BinaryOperator? op = node.Value as BinaryOperator?;
            if (op == null)
                return false;

            var nodePair = new ValueTuple<BinaryOperator, LinkedListNode<object>>(op.Value, node);

            for (var currentNode = _oplist.First; currentNode != null; currentNode = currentNode.Next) {
                if (currentNode.Value.Item1.Precedence > nodePair.Item1.Precedence) {
                    _oplist.AddBefore(currentNode, nodePair);
                    return true;
                }
            }
            _oplist.AddLast(nodePair);
            return true;
        }

        public bool Dequeue(out ValueTuple<BinaryOperator, LinkedListNode<object>> nodePair)
        {
            if (_oplist.Count == 0) {
                nodePair = default(ValueTuple<BinaryOperator, LinkedListNode<object>>);
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
