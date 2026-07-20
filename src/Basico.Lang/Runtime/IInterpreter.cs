namespace Basico.Runtime
{
    public interface IInterpreter
    {
        RunValue? Call(RunValue callable, List<RunValue> arguments);
        bool Run();
    }
}
