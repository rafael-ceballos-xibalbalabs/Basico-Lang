namespace Basico.Ast
{
    public interface IIfStatement
    {
        StatementExpression Condition { get; }
        List<Statement> Then { get; }
        List<Statement> Otherwise { get; }
    }
}
