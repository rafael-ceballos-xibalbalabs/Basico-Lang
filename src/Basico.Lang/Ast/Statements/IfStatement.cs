namespace Basico.Ast
{
    public class IfStatement : Statement, IIfStatement
    {
        public IfStatement(StatementExpression condition, List<Statement> then, List<Statement> otherwise)
        {
            Condition = condition;
            Then = then;
            Otherwise = otherwise;
        }
        public StatementExpression Condition { get; }
        public List<Statement> Then { get; }
        public List<Statement> Otherwise { get; }

        public override string ToString()
        {
            return string.Format("{{ Condition: {0}, Parameters: [{1}], Body: [{2}] }}", Condition, string.Join(", ", Then), string.Join(", ", Otherwise));
        }
    }
}
