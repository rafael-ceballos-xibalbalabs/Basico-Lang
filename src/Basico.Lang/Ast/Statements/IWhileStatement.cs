namespace Basico.Ast
{
    public interface IWhileStatement : IStatement
    {
        StatementExpression Condition { get; }
        List<Statement> Block { get; }
    }
}
