using System.Text;
using static Basico.Runtime.RunValue;

namespace Basico.Runtime
{
    public struct NumberObject
    {

        Common _common = default;
        StringBuilder _sb = new StringBuilder();
        public NumberObject()
        {
            _common = new Common();

        }

        public NativeMethodCallback Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            NativeMethodCallback callback = null;
            switch (name.ToLowerInvariant().Trim())
            {
                case "es_entero": callback = number_es_entero; break;
                case "es_decimal": callback = number_es_decimal; break;
                default: throw new ArgumentException(string.Format("Undefined method: {0}", name));

            }


            return callback;
        }
        public RunValue number_es_entero(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("Number.es_entero", 0, arguments);
            var number = context.to_number();

            return new BooleanRunValue(int.TryParse(number.ToString(), out int n));
        }
        public RunValue number_es_decimal(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("Number.es_decimal", 0, arguments);
            var number = context.to_number();

            return new BooleanRunValue(double.TryParse(number.ToString(), out double n));
        }

    }
}
