namespace Basico.Ast
{
    public class AstProgram
    {
        public List<Statement> Statements { get; set; } = new List<Statement>();
        public override string ToString()
        {
            return string.Format("[ {0} ]", string.Join(", ", Statements));
        }
    }
}
