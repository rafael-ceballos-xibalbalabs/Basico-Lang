namespace Basico.Ast
{
    public class ListIndexExpression : StatementExpression
    {
        public ListIndexExpression(StatementExpression expression, StatementExpression? index)
        {
            Expression = expression;
            Index = index;
        }

        public StatementExpression Expression { get; }
        public StatementExpression? Index { get; }

        public override string ToString()
        {
            return string.Format("{{ Expression: {0}, Index: [{1}] }}", Expression, Index);
        }
    }
}
