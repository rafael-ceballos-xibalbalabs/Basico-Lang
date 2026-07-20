using System.Reflection.Metadata;

namespace Basico.Ast
{
    public class FunctionDeclarationStatement : Statement, IFunctionDeclarationStatement
    {
        public StatementKind Kind => StatementKind.Function;

        public FunctionDeclarationStatement(string name, List<Parameter> parameters, List<Statement> body)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Parameters = parameters;
            Body = body;
        }

        public string Name { get; }

        public List<Parameter> Parameters { get; }

        public List<Statement> Body { get; }

        public override string ToString()
        {
            return string.Format("{{ Name: {0}, Parameters: [{1}], Body: [{2}] }}", Name, string.Join(", ", Parameters), string.Join(", ", Body));
        }
    }
}
