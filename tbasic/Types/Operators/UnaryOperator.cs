// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using System.Diagnostics.Contracts;
using Tbasic.Runtime;

namespace Tbasic.Types
{
    /// <summary>
    /// Represents an operator that takes one operand
    /// </summary>
    public struct UnaryOperator : IOperator, IEquatable<UnaryOperator>
    {
        /// <summary>
        /// A delegate that represents the method which processes the operand. 
        /// </summary>
        /// <param name="runtime">the current runtime</param>
        /// <param name="value">the operand</param>
        /// <returns>the result of the operator</returns>
        public delegate object UnaryOpDelegate(TRuntime runtime, object value);

        /// <summary>
        /// Gets the string representation of the operator
        /// </summary>
        public string OperatorString { get; private set; }

        /// <summary>
        /// Gets the method that processes the operands
        /// </summary>
        public UnaryOpDelegate ExecuteOperator { get; private set; }
        
        /// <summary>
        /// Gets whether or not the operand should be evaluated
        /// </summary>
        public bool EvaluateOperand { get; private set; }

        /// <summary>
        /// Creates a new UnaryOperator
        /// </summary>
        /// <param name="strOp">the string representation of the operator</param>
        /// <param name="doOp">the method that processes the operand</param>
        /// <param name="evaluate">whether or not the operand should be evaluated</param>
        public UnaryOperator(string strOp, UnaryOpDelegate doOp, bool evaluate = true)
        {
            if (strOp == null)
                throw new ArgumentNullException(nameof(strOp));
            if (doOp == null)
                throw new ArgumentNullException(nameof(doOp));
            Contract.EndContractBlock();

            OperatorString = strOp;
            ExecuteOperator = doOp;
            EvaluateOperand = evaluate;
        }

        /// <summary>
        /// Determines if two UnaryOperators are equal
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(UnaryOperator other)
        {
            return OperatorString == other.OperatorString && ExecuteOperator == other.ExecuteOperator && EvaluateOperand == other.EvaluateOperand;
        }

        /// <summary>
        /// Determines if this object is equal to another
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            UnaryOperator? op = obj as UnaryOperator?;
            if (op != null)
                return Equals(op.Value);
            return false;
        }

        /// <summary>
        /// Determines if two operators are equal
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool operator ==(UnaryOperator first, UnaryOperator second)
        {
            return Equals(first, second);
        }

        /// <summary>
        /// Determines if two operators are not equal
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool operator !=(UnaryOperator first, UnaryOperator second)
        {
            return !Equals(first, second);
        }

        /// <summary>
        /// Gets the hash code for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return OperatorString.GetHashCode() ^ ExecuteOperator.GetHashCode() ^ EvaluateOperand.GetHashCode();
        }
    }
}
