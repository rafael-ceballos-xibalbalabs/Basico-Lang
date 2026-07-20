namespace Basico.Ast
{
    public class BooleanExpression : StatementExpression
    {
        public BooleanExpression(bool boolean)
        {
            Boolean = boolean;
        }
        public bool Boolean { get; }

        public override string ToString()
        {
            return string.Format("Bool({0})", Boolean);
        }
    }
}
