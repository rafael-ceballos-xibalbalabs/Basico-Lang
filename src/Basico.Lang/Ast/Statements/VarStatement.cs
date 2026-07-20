namespace Basico.Ast
{
    public class VarStatement : Statement, IVarStatement
    {
        public StatementKind Kind => StatementKind.Var;
        public VarStatement(string name, StatementExpression expression)
        {
            Name = name;
            Expression = expression;
        }
        public string Name { get; }

        public StatementExpression Expression { get; }

        public override string ToString()
        {
            return string.Format("Var {{ name: {0}, expression: {1} }}", Name, Expression.ToString());
        }
    }
}
