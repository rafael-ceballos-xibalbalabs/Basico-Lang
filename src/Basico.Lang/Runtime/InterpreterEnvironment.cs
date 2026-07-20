using System.Diagnostics;

namespace Basico.Runtime
{
    public class InterpreterEnvironment : IInterpreterEnvironment
    {
        public Dictionary<string, RunValue> Values { get; }

        public InterpreterEnvironment()
        {
            Values = new Dictionary<string, RunValue>();
        }

        public void Set(string key, RunValue value)
        {
            if (!Values.TryAdd(key, value))
            {
                throw new InvalidOperationException(string.Format("Unit interpretation error. Could not set key: {0} value: {1}", key, value));
            }
        }

        public void Assign(string key, RunValue value)
        {
            Values[key] = value;
        }

        public RunValue? Get(string key)
        {
            Values.TryGetValue(key, out RunValue value);
            return value ?? default;
        }

        public void Drop(string key)
        {
            Values.Remove(key);
        }

        public void Dump()
        {
            Debug.WriteLine(Values);
        }
    }
}
