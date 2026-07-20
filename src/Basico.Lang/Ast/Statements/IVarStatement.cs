namespace Basico.Ast
{
    public interface IVarStatement : IStatement
    {
        string Name { get; }
        StatementExpression Expression { get; }
    }
}
