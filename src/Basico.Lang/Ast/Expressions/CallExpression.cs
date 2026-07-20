namespace Basico.Ast
{
    public class CallExpression : StatementExpression
    {
        public CallExpression(StatementExpression expression, List<StatementExpression> args)
        {
            Expression = expression;
            Args = args;
        }
        public StatementExpression Expression { get; }
        public List<StatementExpression> Args { get; }

        public override string ToString()
        {
            return string.Format("{{ Expression: {0}, Args: [{1}] }}", Expression, string.Join(", ", Args));
        }
    }
}
