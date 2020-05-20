// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;
using Tbasic.Runtime;
using Tbasic.Types;

namespace Tbasic.Parsing
{
    public partial class DefaultScanner
    {
        /// <summary>
        /// Tries to match the next token in the buffer as a BinaryOperator
        /// </summary>
        /// <param name="context">the context that contains the operators</param>
        /// <param name="foundOp">contains the operator that was found</param>
        /// <returns>true if the token could be matched, false otherwise</returns>
        public bool NextBinaryOp(ObjectContext context, out BinaryOperator foundOp)
        {
            foundOp = default(BinaryOperator);
            string token = BuffNextWord();
            if (string.IsNullOrEmpty(token))
                return false;

            if (MatchBinaryOperator(token, context, out foundOp)) {
                AdvanceScanner(foundOp.OperatorString.Length);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to match the next token in the buffer as a UnaryOperator
        /// </summary>
        /// <param name="context">the context that contains the operators</param>
        /// <param name="foundOp">contains the operator that was found</param>
        /// <returns>true if the token could be matched, false otherwise</returns>
        /// <param name="last">the last token that was matched or null if there is no previous token</param>
        public bool NextUnaryOp(ObjectContext context, object last, out UnaryOperator foundOp)
        {
            foundOp = default(UnaryOperator);
            if (!(last == null || last is BinaryOperator)) // unary operators can really only come after a binary operator or the beginning of the expression
                return false;

            string token = BuffNextWord();
            if (string.IsNullOrEmpty(token))
                return false;

            if (MatchUnaryOperator(token, context, out foundOp)) {
                AdvanceScanner(foundOp.OperatorString.Length);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Tries to match a token as a BinaryOperator
        /// </summary>
        /// <param name="token">the token to match</param>
        /// <param name="context">the context that contains the operators</param>
        /// <param name="foundOp">contains the operator that was found</param>
        /// <returns>true if the token could be matched, false otherwise</returns>
        public static bool MatchBinaryOperator(string token, ObjectContext context, out BinaryOperator foundOp)
        {
            string foundStr = string.Empty;
            foundOp = default(BinaryOperator);
            var ops = context.GetAllBinaryOperators();
            for(int i = 0; i < ops.Count; ++i) {
                string opStr = ops[i].OperatorString;
                if (foundStr.Length < opStr.Length && token.StartsWith(opStr, StringComparison.OrdinalIgnoreCase)) {
                    foundOp = ops[i];
                    foundStr = opStr;
                }
            }
            return foundStr != string.Empty;
        }

        /// <summary>
        /// Tries to match a token as a UnaryOperator
        /// </summary>
        /// <param name="token">the token to match</param>
        /// <param name="context">the context that contains the operators</param>
        /// <param name="foundOp">contains the operator that was found</param>
        /// <returns>true if the token could be matched, false otherwise</returns>
        public static bool MatchUnaryOperator(string token, ObjectContext context, out UnaryOperator foundOp)
        {
            string foundStr = string.Empty;
            foundOp = default(UnaryOperator);
            var ops = context.GetAllUnaryOperators();
            for(int i = 0; i < ops.Count; ++i) {
                string opStr = ops[i].OperatorString;
                if (foundStr.Length < opStr.Length && token.StartsWith(opStr, StringComparison.OrdinalIgnoreCase)) {
                    foundOp = ops[i];
                    foundStr = opStr;
                }
            }
            return foundStr != string.Empty;
        }
    }
}

