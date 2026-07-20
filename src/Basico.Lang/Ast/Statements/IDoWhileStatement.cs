namespace Basico.Ast
{
    public interface IDoWhileStatement : IStatement
    {
        StatementExpression Condition { get; }
        List<Statement> Block { get; }
    }
}
