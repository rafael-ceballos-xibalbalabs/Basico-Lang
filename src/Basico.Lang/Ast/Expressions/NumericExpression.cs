namespace Basico.Ast
{
    public class NumericExpression : StatementExpression
    {
        public NumericExpression(double literal)
        {
            Literal = literal;
        }

        public double Literal { get; }

        public override string ToString()
        {
            return string.Format("Number({0})", Literal);
        }
    }
}
