namespace Basico.Lexing
{
    internal record Token(TokenKind kind, object literal)
    {
        public override string ToString()
        {
            return string.Format("Token {{ kind: {0}, literal: \"{1}\" }}, ", kind, literal);
        }
    }
}
