namespace Basico.Ast
{
    public class ReturnStatement : Statement, IExpressionStatement
    {
        public StatementKind Kind => StatementKind.Expression;
        public ReturnStatement(StatementExpression expression)
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
