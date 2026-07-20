namespace Basico.Ast
{
    public class StringExpression : StatementExpression
    {
        public StringExpression(string literal)
        {
            Literal = literal ?? string.Empty;
        }
        public string Literal { get; }
        public override string ToString()
        {
            return string.Format("String({0})", Literal);
        }

    }
}
