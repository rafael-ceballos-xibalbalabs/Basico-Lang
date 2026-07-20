using System.Reflection.Metadata;

namespace Basico.Ast
{
    public class StructDeclarationStatement : Statement, IStructDeclarationStatement
    {
        public StatementKind Kind => StatementKind.Struct;

        public StructDeclarationStatement(string name, List<Parameter> fields)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Fields = fields;
        }

        public string Name { get; }
        public List<Parameter> Fields { get; }
        public override string ToString()
        {
            return string.Format("{{ Name: {0}, Fields: [{1}]}}", Name, string.Join(", ", Fields));
        }
    }
}
