namespace Basico.Runtime
{
    public interface IInterpreterEnvironment
    {
        void Set(string key, RunValue value);
        void Assign(string key, RunValue value);
        RunValue? Get(string key);
        void Drop(string key);
        void Dump();
    }
}
