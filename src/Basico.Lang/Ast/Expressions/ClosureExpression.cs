using System.Reflection.Metadata;

namespace Basico.Ast
{
    public class ClosureExpression : StatementExpression
    {
        public ClosureExpression(List<Parameter> parameters, List<Statement> body)
        {
            Parameters = parameters;
            Body = body;
        }

        public List<Parameter> Parameters { get; }
        public List<Statement> Body { get; }
        public override string ToString()
        {
            return string.Format("{{ Parameters: [{0}], Body: [{1}] }}", string.Join(", ", Parameters), string.Join(", ", Body));
        }
    }
}
