namespace Basico.Ast
{
    public class AssignmentExpression : StatementExpression
    {
        public AssignmentExpression(StatementExpression left, StatementExpression right)
        {
            Left = left;
            Right = right;
        }

        public StatementExpression Left { get; }
        public StatementExpression Right { get; }

        public override string ToString()
        {
            return string.Format("{{ Left: {0}, Right: {1} }}", Left, Right);
        }
    }
}
