namespace Basico.Parsing
{
    public class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException()
        {
        }

        public UnexpectedTokenException(string? message) : base(message)
        {
        }

        public UnexpectedTokenException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
