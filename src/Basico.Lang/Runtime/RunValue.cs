using Basico.Ast;

namespace Basico.Runtime
{
    public abstract class RunValue
    {
        public IEnumerable<RunValue> to_list()
        {
            if (this is ListRunValue listRunValue)
            {
                return listRunValue.Values;
            }

            throw new NotImplementedException();
        }

        public double to_number()
        {
            if (this is NumberRunValue numberRunValue)
            {
                return (double)numberRunValue.Value;
            }
            else if (this is BooleanRunValue booleanRunValue)
            {
                return booleanRunValue.Value ? 1 : 0;
            }
            else if (this is NullRunValue)
            {
                return 0;
            }
            else if (this is StringRunValue stringRunValue)
            {
                if (!double.TryParse(stringRunValue?.Value.Trim(), out double result)) return 0;
                return result;
            }
            return 0;
        }
        public string to_string()
        {
            if (this is StringRunValue stringRunValue)
            {
                return stringRunValue.Value;
            }
            else if (this is NumberRunValue numberRunValue)
            {
                return numberRunValue.Value.ToString();
            }
            else if (this is BooleanRunValue booleanRunValue)
            {
                return booleanRunValue.ToString() ?? string.Empty;
            }
            else if (this is NullRunValue)
            {
                return string.Empty;
            }
            else if (this is ListRunValue lrv)
            {
                return lrv.Values.ToDelimitedString();
            }
            else
            {
                return this.ToString();
            }
        }
        public bool to_bool()
        {
            if (this is BooleanRunValue booleanRunValue && booleanRunValue.Value || this is FunctionRunValue)
            {
                return true;
            }
            else if (this is StringRunValue s)
            {
                return !string.IsNullOrWhiteSpace(s.Value);
            }
            else if (this is NumberRunValue numberRunValue)
            {
                return numberRunValue.Value > 0.0;
            }
            //constante
            else
            {
                return false;
            }
        }

        public string typestring()
        {
            if (this is StringRunValue stringRunValue)
            {
                return stringRunValue.Value;
            }
            else if (this is NumberRunValue numberRunValue)
            {
                return numberRunValue.Value.ToString();
            }
            else if (this is BooleanRunValue booleanRunValue)
            {
                return booleanRunValue.ToString() ?? string.Empty;
            }
            else if (this is NullRunValue)
            {
                return string.Empty;
            }
            else
            {
                return this.ToString();
            }
        }
        public class NumberRunValue : RunValue
        {
            public NumberRunValue(double value)
            {
                Value = value;
            }
            public double Value { get; }
        }

        public class BooleanRunValue : RunValue
        {
            public BooleanRunValue(bool value)
            {
                Value = value;
            }

            public bool Value { get; }
        }
        public class StringRunValue : RunValue
        {
            public StringRunValue(string value)
            {
                Value = value;
            }
            public string Value { get; }

            public override string ToString()
            {
                return Value;
            }
        }
        public class NullRunValue : RunValue
        {
            public NullRunValue()
            {
            }
            public object Value { get; set; } = null;
        }
        public class FunctionRunValue : RunValue
        {
            public FunctionRunValue(string name, List<Parameter> parameters, List<Statement> block, InterpreterEnvironment? environment, StatementExpression? context)
            {
                Name = name;
                Parameters = parameters;
                Block = block;
                Environment = environment;
                Context = context;
            }

            public string Name { get; }
            public List<Parameter> Parameters { get; }
            public List<Statement> Block { get; }
            public InterpreterEnvironment? Environment { get; }
            public StatementExpression? Context { get; }
        }
        public class StructRunValue : RunValue
        {
            public StructRunValue(string name, List<Parameter> fields, Dictionary<string, RunValue> methods)
            {
                Name = name;
                Fields = fields;
                Methods = methods;
            }

            public string Name { get; }
            public List<Parameter> Fields { get; }
            public Dictionary<string, RunValue> Methods { get; }
        }
        public class StructInstanceRunValue : RunValue
        {
            public StructInstanceRunValue(InterpreterEnvironment? environment, RunValue? definition)
            {
                Environment = environment;
                Definition = definition;
            }
            public InterpreterEnvironment? Environment { get; }
            public RunValue? Definition { get; }
        }
        public class ListRunValue : RunValue
        {
            public ListRunValue(List<RunValue> values)
            {
                Values = values;
            }

            public List<RunValue> Values { get; }
        }
        public delegate RunValue NativeFunctionCallback(Interpreter interpreter, List<RunValue> values);
        public delegate RunValue NativeMethodCallback(Interpreter interpreter, RunValue value, List<RunValue> values);

        public class NativeFunctionRunValue : RunValue
        {
            public NativeFunctionRunValue(string name, NativeFunctionCallback callback)
            {
                Name = name;
                Callback = callback;
            }
            public string Name { get; }
            public NativeFunctionCallback Callback { get; }
        }
        public class NativeMethodRunValue : RunValue
        {
            public NativeMethodRunValue(string name, NativeMethodCallback callback, StatementExpression context)
            {
                Name = name;
                Callback = callback;
                Context = context;
            }

            public string Name { get; }
            public NativeMethodCallback Callback { get; }
            public StatementExpression Context { get; }
        }
    }
}
