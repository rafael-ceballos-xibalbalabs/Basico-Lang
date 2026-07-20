namespace Basico.Ast
{
    public class InfixExpression : StatementExpression
    {
        public InfixExpression(StatementExpression left, Op operand, StatementExpression right)
        {
            Left = left;
            Operand = operand;
            Right = right;
        }

        public StatementExpression Left { get; }
        public Op Operand { get; }
        public StatementExpression Right { get; }

        public override string ToString()
        {
            return string.Format("{{ Left: {0}, Op: {1}, Right: {2} }}", Left, Operand, Right);
        }
    }
}
