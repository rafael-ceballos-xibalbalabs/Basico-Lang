namespace Basico.Ast
{
    public interface IForEachStatement : IStatement
    {
        StatementExpression Iterable { get; }
        string Identifier { get; }
        string? Index { get; }
        List<Statement> Block { get; }
    }
}
