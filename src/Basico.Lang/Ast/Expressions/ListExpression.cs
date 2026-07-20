namespace Basico.Ast
{
    public class ListExpression : StatementExpression
    {
        public ListExpression(List<StatementExpression> items)
        {
            Items = items;
        }

        public List<StatementExpression> Items { get; }
        public override string ToString()
        {
            return string.Format("[ Items: {0} ]", string.Join(", ", Items));
        }
    }
}
