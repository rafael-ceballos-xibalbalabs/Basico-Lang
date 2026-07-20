namespace Basico.Ast
{
    public interface IExpressionStatement : IStatement
    {
        StatementExpression Expression { get; }
    }
}
