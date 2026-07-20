namespace Basico.Ast
{
    public class ForEachStatement : Statement, IForEachStatement
    {
        public StatementKind Kind => StatementKind.ForEach;

        public ForEachStatement(StatementExpression iterable, string value, string? index, List<Statement> block)
        {
            Iterable = iterable;
            Value = value;
            Index = index;
            Block = block;
        }

        public StatementExpression Iterable { get; }
        public string Value { get; }
        public string Identifier { get; }

        public string? Index { get; }

        public List<Statement> Block { get; }

    }
}
