namespace Basico.Ast
{
    public class IdentifierExpression : StatementExpression
    {
        public IdentifierExpression(string identifier)
        {
            Identifier = identifier;
        }
        public string Identifier { get; }
        public override string ToString()
        {
            return string.Format("Identifier({0})", Identifier);
        }
    }
}
