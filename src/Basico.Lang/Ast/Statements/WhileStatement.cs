namespace Basico.Ast
{
    public class WhileStatement : Statement, IWhileStatement
    {
        public StatementKind Kind => StatementKind.While;

        public WhileStatement(StatementExpression condition, List<Statement> block)
        {
            Condition = condition;
            Block = block;
        }

        public StatementExpression Condition { get; }
        public List<Statement> Block { get; }
    }
}
