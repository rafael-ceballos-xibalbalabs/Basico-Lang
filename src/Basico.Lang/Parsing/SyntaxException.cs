namespace Basico.Parsing
{
    public class SyntaxException : Exception
    {
        public SyntaxException(string? message) : base(message)
        {
        }

        public SyntaxException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
