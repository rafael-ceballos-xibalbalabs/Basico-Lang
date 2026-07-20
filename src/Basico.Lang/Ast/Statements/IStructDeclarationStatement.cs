using System.Reflection.Metadata;

namespace Basico.Ast
{
    public interface IStructDeclarationStatement : IStatement
    {
        string Name { get; }
        List<Parameter> Fields { get; }
    }
}
