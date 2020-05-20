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
    /// Represents an operator that takes two operands
    /// </summary>
    public struct BinaryOperator : IOperator, IComparable<BinaryOperator>, IEquatable<BinaryOperator>
    {
        /// <summary>
        /// A delegate that represents the method which processes the operands
        /// </summary>
        /// <param name="runtime">the current runtime</param>
        /// <param name="left">the left operand</param>
        /// <param name="right">the right operand</param>
        /// <returns>the result of the operator</returns>
        public delegate object BinaryOpDelegate(TRuntime runtime, object left, object right);

        /// <summary>
        /// Gets which operand should be evaluated
        /// </summary>
        public OperandPosition EvaulatedOperand { get; private set; }

        /// <summary>
        /// Gets the string representation of the operator
        /// </summary>
        public string OperatorString { get; private set; }

        /// <summary>
        /// Gets the operator precedence. Lower precedence operators are processed first
        /// </summary>
        public int Precedence { get; private set; }

        /// <summary>
        /// Gets the method that processes the operands
        /// </summary>
        public BinaryOpDelegate ExecuteOperator { get; private set; }

        /// <summary>
        /// Creates a new BinaryOperator
        /// </summary>
        /// <param name="strOp">the string representation of the operator</param>
        /// <param name="precedence">the operator precedence. Lower precedence are processed first</param>
        /// <param name="doOp">the method that processes the operands</param>
        /// <param name="operand">the operand that should be evaluated</param>
        public BinaryOperator(string strOp, int precedence, BinaryOpDelegate doOp, OperandPosition operand = OperandPosition.Both)
        {
            if (strOp == null)
                throw new ArgumentNullException(nameof(strOp));
            if (doOp == null)
                throw new ArgumentNullException(nameof(doOp));
            Contract.EndContractBlock();

            OperatorString = strOp;
            Precedence = precedence;
            ExecuteOperator = doOp;
            EvaulatedOperand = operand;
        }
        
        /// <summary>
        /// Compares this operator to another
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(BinaryOperator other)
        {
            return Precedence.CompareTo(other.Precedence);
        }

        /// <summary>
        /// Determines if this operator is equal to another
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(BinaryOperator other)
        {
            return OperatorString == other.OperatorString && Precedence == other.Precedence && EvaulatedOperand == other.EvaulatedOperand;
        }

        /// <summary>
        /// Determines if two operators are equal
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool operator ==(BinaryOperator first, BinaryOperator second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Determines if two operators are not equal
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool operator !=(BinaryOperator first, BinaryOperator second)
        {
            return !first.Equals(second);
        }

        /// <summary>
        /// Determines if this object is equal to another
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            BinaryOperator? op = obj as BinaryOperator?;
            if (op != null)
                return Equals(op.Value);
            return false;
        }

        /// <summary>
        /// Gets the hash code for this object
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return OperatorString.GetHashCode() ^ Precedence ^ ExecuteOperator.GetHashCode();
        }

        /// <summary>
        /// Converts this operator to a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return OperatorString;
        }
    }
}
