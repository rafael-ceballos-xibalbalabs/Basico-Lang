namespace Basico.Ast
{
    public class PrefixExpression : StatementExpression
    {
        public PrefixExpression(Op op, StatementExpression expression)
        {
            Op = op;
            Expression = expression;
        }

        public Op Op { get; }
        public StatementExpression Expression { get; }

        public override string ToString()
        {
            return string.Format("{{ Op: {0}, Expression: {1} }}", Op, Expression);
        }


    }
}
