namespace Basico.Ast
{
    public class ForStatement : Statement, IForStatement
    {
        public StatementKind Kind => StatementKind.For;

        public ForStatement(Statement init, StatementExpression condition, Statement post, List<Statement> block)
        {
            Init = init;
            Condition = condition;
            Post = post;
            Block = block;
        }

        public Statement Init { get; }
        public StatementExpression Condition { get; }
        public Statement Post { get; }
        public List<Statement> Block { get; }
    }
}
