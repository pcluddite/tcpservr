// ======
//
// Copyright (c) Timothy Baxendale. All Rights Reserved.
//
// ======
using System;

namespace Tbasic.Errors
{
    internal class ThrowHelper
    {
        public static Exception UndefinedObject(string name)
        {
            return new UndefinedObjectException($"'{name}' is not defined in the current scope");
        }

        public static Exception UndefinedFunctionOrCommand(string name)
        {
            return new NotImplementedException($"'{name}' is not defined as a command or function");
        }

        public static Exception UndefinedFunction(string name)
        {
            return UndefinedObject($"{name}()");
        }

        public static Exception UnterminatedGroup()
        {
            return new FormatException("Unterminated group");
        }

        public static Exception UnterminatedString()
        {
            return new FormatException("Unterminated string");
        }

        public static Exception UnterminatedEscapeSequence()
        {
            return new FormatException("Unterminated escape sequence");
        }

        public static Exception UnknownEscapeSequence(char escape)
        {
            return new FormatException($"Unknown escape sequence \\{escape}");
        }

        public static Exception UnterminatedUnicodeEscape()
        {
            return new FormatException("Unterminated escape sequence. Expected four digit hex to follow '\\u'");
        }

        public static Exception AlreadyDefinedAsType(string name, string type, string newType)
        {
            return new InvalidCastException($"An object '{name}' has been defined as a {type} and cannot be redefined as a {newType}");
        }

        public static Exception AlreadyDefined(string name)
        {
            return new DuplicateDefinitionException(name);
        }

        public static Exception ConstantChange()
        {
            return new InvalidOperationException("Cannot redefine a constant");
        }

        public static Exception ContextCleared()
        {
            return new InvalidOperationException("Context fell out of scope and was disposed");
        }

        public static Exception InvalidOperator(char opr)
        {
            return new InvalidOperatorException(opr.ToString());
        }

        public static Exception InvalidVariableName(string name)
        {
            return new ScriptParsingException($"The variable name '{name}' contains invalid characters");
        }

        public static Exception InvalidVariableName()
        {
            return new ScriptParsingException("The variable name contains invalid characters");
        }

        public static Exception ArraysCannotBeConstant()
        {
            return new ScriptParsingException("Arrays cannot be defined as constants");
        }

        public static Exception ExpectedToken(string token)
        {
            return new ExpectedTokenExceptiopn(token);
        }

        public static Exception NoOpeningStatement(string str)
        {
            return new ScriptParsingException($"Cannot find opening statement for '{str}'");
        }

        public static Exception NoCondition()
        {
            return new ScriptParsingException("Expected condition");
        }

        public static Exception UnterminatedBlock(string name)
        {
            return new EndOfCodeException($"Unterminated '{name}' block");
        }

        public static Exception InvalidTypeInExpression(string expr, string expected)
        {
            return new ScriptParsingException($"Invalid type in expression '{expr ?? "null"}', expected '{expected}'");
        }

        public static Exception NoIndexSpecified()
        {
            return new FormatException("At least one index was expected between braces");
        }

        public static Exception IndexUnavailable(string name)
        {
            return new InvalidOperationException($"Object '{name}' cannot be indexed");
        }

        public static Exception IndexOutOfRange(string name, int index)
        {
            return new InvalidOperationException($"Index '{index}' of object '{name}' is out of range");
        }

        public static Exception InvalidExpression(string expr)
        {
            return new ScriptParsingException($"Invalid expression [{expr}]");
        }

        public static Exception ExpectedSpaceAfterCommand()
        {
            return new FormatException("Expected space after command name");
        }

        public static Exception OperatorUndefined(string opr)
        {
            return new ArgumentException($"Operator [{opr}] is undefined");
        }

        public static Exception MacroRedefined()
        {
            return new ArgumentException("Cannot redefine a macro");
        }

        public static Exception InvalidDefinitionOperator()
        {
            return new ArgumentException("Expected [=] in definition");
        }

        public static Exception MissingBinaryOp(object left, object right)
        {
            return new ArgumentException($"Missing binary operator - {QuoteIfString(left)} [?] {QuoteIfString(right)}");
        }

        private static string QuoteIfString(object str)
        {
            if (str is string) {
                return $"\"{str}\"";
            }
            else {
                return str?.ToString();
            }
        }

        public static Exception InvalidParamType(int index, string rightType)
        {
            return new InvalidCastException($"Expected parameter {index} to be of type {rightType}");
        }
    }
}
