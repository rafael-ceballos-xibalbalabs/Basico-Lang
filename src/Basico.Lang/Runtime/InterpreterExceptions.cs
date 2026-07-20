using System.Runtime.Serialization;

namespace Basico.Runtime
{
    public static class InterpreterExceptions
    {
        //#[error("")]
        //        Return(Value),
        public class ReturnValueException : Exception
        {

        }

        public class UndefinedVariableException : Exception
        {
            public UndefinedVariableException(string? message) : base(message)
            {
            }

            public UndefinedVariableException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }

        public class UndefinedIndexException : Exception
        {
            public UndefinedIndexException(string? message) : base(message)
            {
            }

            public UndefinedIndexException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
        public class UndefinedFieldException : Exception
        {
            public UndefinedFieldException(string? message) : base(message)
            {
            }

            public UndefinedFieldException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
        public class UndefinedMethodException : Exception
        {
            public UndefinedMethodException(string? message) : base(message)
            {
            }

            public UndefinedMethodException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
        public class InvalidIterableException : Exception
        {
            public InvalidIterableException(string? message) : base(message)
            {
            }

            public InvalidIterableException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }

        public class TooFewArgumentsException : Exception
        {
            public TooFewArgumentsException(string? message) : base(message)
            {
            }

            public TooFewArgumentsException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
        public class InvalidAppendTargetException : Exception
        {
            public InvalidAppendTargetException(string? message) : base(message)
            {
            }

            public InvalidAppendTargetException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }
        public class MissingArgumentException : Exception
        {

        }
        public class InvalidMethodAssignmentException : Exception
        {
            public InvalidMethodAssignmentException(string? message) : base(message)
            {
            }

            public InvalidMethodAssignmentException(string? message, Exception? innerException) : base(message, innerException)
            {
            }
        }

        [Serializable]
        internal class ExpectedIndexException : Exception
        {
            public ExpectedIndexException()
            {
            }

            public ExpectedIndexException(string? message) : base(message)
            {
            }

            public ExpectedIndexException(string? message, Exception? innerException) : base(message, innerException)
            {
            }

            protected ExpectedIndexException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        [Serializable]
        internal class ExpressionEvaluationException : Exception
        {
            public ExpressionEvaluationException()
            {
            }

            public ExpressionEvaluationException(string? message) : base(message)
            {
            }

            public ExpressionEvaluationException(string? message, Exception? innerException) : base(message, innerException)
            {
            }

            protected ExpressionEvaluationException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        [Serializable]
        internal class UnitCompilationException : Exception
        {
            public UnitCompilationException()
            {
            }

            public UnitCompilationException(string? message) : base(message)
            {
            }

            public UnitCompilationException(string? message, Exception? innerException) : base(message, innerException)
            {
            }

            protected UnitCompilationException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }

        [Serializable]
        internal class LoadModuleException : Exception
        {
            public LoadModuleException()
            {
            }

            public LoadModuleException(string? message) : base(message)
            {
            }

            public LoadModuleException(string? message, Exception? innerException) : base(message, innerException)
            {
            }

            protected LoadModuleException(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}
