namespace Basico.Ast
{
    public interface IForStatement : IStatement
    {
        Statement Init { get; }
        StatementExpression Condition { get; }
        Statement Post { get; }
        List<Statement> Block { get; }
    }
}
