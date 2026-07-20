using System.Text;
using static Basico.Runtime.RunValue;

namespace Basico.Runtime
{
    public struct ListObject
    {
        Common _common = default;
        StringBuilder _sb = new StringBuilder();
        public ListObject()
        {
            _common = new Common();

        }

        public NativeMethodCallback Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            NativeMethodCallback callback = null;
            switch (name.ToLowerInvariant().Trim())
            {
                case "esta_vacia": callback = list_esta_vacia; break;
                case "invertir": callback = list_invertir; break;
                case "unir": callback = list_unir; break;
                case "filtrar": callback = list_filtrar; break;
                case "agregar": callback = list_agregar; break;
                case "para_cada": callback = list_para_cada; break;
                case "transformar": callback = list_transformar; break;
                case "primero": callback = list_primero; break;
                case "ultimo": callback = list_ultimo; break;
                //case "get": callback = list_get; break;

                default: throw new ArgumentException(string.Format("Undefined method: {0}", name));

            }


            return callback;
        }

        public RunValue list_esta_vacia(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("List.esta_vacia()", 0, arguments);

            return new BooleanRunValue(!context.to_list().Any());
        }
        public RunValue list_invertir(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("List.invertir()", 0, arguments);
            var list = context.to_list().Reverse().ToList();

            return new ListRunValue(list);
        }
        public RunValue list_unir(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("List.unir()", 1, arguments);
            var list = context.to_list().ToList();
            var separator = arguments?.FirstOrDefault()?.to_string();

            var result = string.Join(separator, list.Select(x => x.to_string()).ToList());

            return new StringRunValue(result);
        }
        public RunValue list_filtrar(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("List.filtrar()", 1, arguments);

            var callback = arguments?.FirstOrDefault();
            var list = new List<RunValue>();

            foreach (var item in context.to_list().ToList())
            {
                var ls = new List<RunValue>();
                ls.Add(item);
                if (interpreter.Call(callback, ls).to_bool())
                {
                    list.Add(item);
                }

            }

            return new ListRunValue(list);
        }
        public RunValue list_para_cada(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("List.para_cada()", 1, arguments);

            var callback = arguments?.FirstOrDefault();
            var list = new List<RunValue>();

            foreach (var item in context.to_list().ToList())
            {
                interpreter.Call(callback, new List<RunValue> { item }).to_bool();
            }

            return context;
        }
        public RunValue list_transformar(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("List.transformar()", 1, arguments);

            var callback = arguments?.FirstOrDefault();
            var list = context.to_list().ToList();

            for (int i = 0; i < list.Count; i++)
            {
                var res = interpreter.Call(callback, new List<RunValue> { list[i] });
                list[i] = res;
            }

            return new ListRunValue(list);
        }
        public RunValue list_primero(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            //_common.arity("List.transformar()", 1, arguments);


            var list = context.to_list().ToList();

            if (!list.Any())
            {
                return new NullRunValue();
            }
            if (arguments.Count == 1)
            {
                var callback = arguments?.FirstOrDefault();
                foreach (var item in list)
                {
                    var result = interpreter.Call(callback, new List<RunValue> { item });
                    if (result.to_bool()) return item;
                }

            }


            return list.First();
        }
        public RunValue list_ultimo(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            //_common.arity("List.transformar()", 1, arguments);
            var list = context.to_list().Reverse().ToList();

            if (!list.Any())
            {
                return new NullRunValue();
            }
            if (arguments.Count == 1)
            {
                var callback = arguments?.FirstOrDefault();
                foreach (var item in list)
                {
                    var result = interpreter.Call(callback, new List<RunValue> { item });
                    if (result.to_bool()) return item;
                }
            }
            return list.First();
        }

        public RunValue list_agregar(Interpreter interpreter, RunValue context, List<RunValue> arguments)
        {
            _common.arity("List.agregar()", 1, arguments);

            var callback = arguments?.FirstOrDefault();
            var list = new List<RunValue>();

            foreach (var item in context.to_list().ToList())
            {
                var ls = new List<RunValue>();
                ls.Add(item);
                var r = interpreter.Call(callback, ls);
                list.Add(item);

            }
            return new ListRunValue(list);
        }
    }
}
