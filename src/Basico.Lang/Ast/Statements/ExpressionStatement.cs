namespace Basico.Ast
{
    public class ExpressionStatement : Statement, IExpressionStatement
    {
        public StatementKind Kind => StatementKind.Expression;
        public ExpressionStatement(StatementExpression expression)
        {
            Expression = expression;
        }

        public StatementExpression Expression { get; }

        public override string ToString()
        {
            return string.Format("Expression {{ Expression: {0} }}", Expression);
        }
    }
}
