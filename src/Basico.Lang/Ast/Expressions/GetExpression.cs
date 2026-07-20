namespace Basico.Ast
{
    public class GetExpression : StatementExpression
    {
        public GetExpression(StatementExpression expression, string field)
        {
            Expression = expression;
            Field = field;
        }

        public StatementExpression Expression { get; }
        public string Field { get; }

        public override string ToString()
        {
            return string.Format("{{ Expression: {0}, Field: {1} }}", Expression, Field);
        }
    }
}
