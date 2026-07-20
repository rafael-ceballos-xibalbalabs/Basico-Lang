namespace Basico.Ast
{
    public class StructExpression : StatementExpression
    {
        public StructExpression(StatementExpression expression, Dictionary<string, StatementExpression> fields)
        {
            Expression = expression;
            Fields = fields;
        }

        public StatementExpression Expression { get; }
        public Dictionary<string, StatementExpression> Fields { get; }

        public override string ToString()
        {
            return string.Format("{{ Expression: {0}, Fields: [{1}] }}", Expression, Fields);
        }
    }
}
