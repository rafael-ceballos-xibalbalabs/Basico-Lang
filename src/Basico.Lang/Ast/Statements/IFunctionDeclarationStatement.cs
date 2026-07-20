using System.Reflection.Metadata;

namespace Basico.Ast
{
    public interface IFunctionDeclarationStatement : IStatement
    {
        string Name { get; }
        List<Parameter> Parameters { get; }
        List<Statement> Body { get; }
    }
}
