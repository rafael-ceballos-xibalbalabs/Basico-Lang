namespace Basico.Ast
{
    public class DoWhileStatement : Statement, IDoWhileStatement
    {
        public StatementKind Kind => StatementKind.DoWhile;

        public DoWhileStatement(StatementExpression condition, List<Statement> block)
        {
            Condition = condition;
            Block = block;
        }

        public StatementExpression Condition { get; }
        public List<Statement> Block { get; }
    }
}
