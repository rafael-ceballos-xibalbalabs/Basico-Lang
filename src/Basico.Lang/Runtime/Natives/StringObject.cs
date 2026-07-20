using System.Text;
using static Basico.Runtime.RunValue;

namespace Basico.Runtime
{
    public struct StringObject
    {
        Common _common = default;
        StringBuilder _sb = new StringBuilder();
        public StringObject()
        {
            _common = new Common();

        }

        public NativeMethodCallback Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            NativeMethodCallback callback = null;
            switch (name.ToLower().Trim())
            {
                case "contiene": callback = string_contiene; break;
                case "empieza_con": callback = string_empieza_con; break;
                case "termina_con": callback = string_termina_con; break;
                case "finalizar": callback = string_finalizar; break;
                case "agregar": callback = string_agregar; break;
                case "inspeccionar": callback = string_inspeccionar; break;
                case "a_mayusculas": callback = string_a_mayusculas; break;
                case "a_minusculas": callback = string_a_minusculas; break;
                case "es_blanco": callback = string_es_blanco; break;

                default: throw new ArgumentException(string.Format("Undefined method: {0}", name));

            }


            return callback;
        }

        public RunValue string_contiene(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("String.contiene", 1, arguments);

            var myString = context.to_string();
            foreach (var arg in arguments)
            {
                if (myString.Contains(arg.to_string())) return new BooleanRunValue(true);
            }
            return new BooleanRunValue(false);
        }
        public RunValue string_empieza_con(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("String.empieza_con", 1, arguments);

            var myString = context.to_string();
            foreach (var arg in arguments)
            {
                if (myString.StartsWith(arg.to_string())) return new BooleanRunValue(true);
            }
            return new BooleanRunValue(false);
        }
        public RunValue string_termina_con(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("String.termina_con", 1, arguments);

            var myString = context.to_string();
            foreach (var arg in arguments)
            {
                if (myString.EndsWith(arg.to_string())) return new BooleanRunValue(true);
            }
            return new BooleanRunValue(false);
        }
        public RunValue string_finalizar(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _sb.Clear();
            _common.arity("String.finalizar", 1, arguments);

            var myString = context.to_string();
            var append = arguments?.FirstOrDefault()?.to_string();
            _sb.Append(myString);
            if (!myString.EndsWith(append)) _sb.Append(append);
            return new StringRunValue(_sb.ToString());
        }
        public RunValue string_agregar(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _sb.Clear();
            _common.arity("String.agregar", 1, arguments);

            var myString = context.to_string();
            var append = arguments?.FirstOrDefault()?.to_string();
            _sb.Append(myString);
            _sb.Append(append);
            return new StringRunValue(_sb.ToString());
        }
        public RunValue string_inspeccionar(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {



            return new BooleanRunValue(false);
        }
        public RunValue string_a_mayusculas(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("String.a_mayusculas", 0, arguments);
            return new StringRunValue(context.to_string().ToUpper());
        }
        public RunValue string_a_minusculas(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("String.a_minusculas", 0, arguments);
            return new StringRunValue(context.to_string().ToLower());
        }
        public RunValue string_es_blanco(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("String.es_blanco", 0, arguments);

            var myString = context.to_string();

            if (string.IsNullOrWhiteSpace(myString)) return new BooleanRunValue(true);

            return new BooleanRunValue(false);
        }
    }
}
