using Basico.Lexing;
using Basico.Parsing;
using static Basico.Runtime.RunValue;

namespace Basico.Runtime
{
    public struct Common
    {
        public void arity(string name, uint arity, List<RunValue> arguments)
        {
            if (arity != arguments.Count) throw new ArgumentException(string.Format("Method {0} expected {1} arguments, received {2}.", name, arity, arguments.Count));
        }
        public RunValue imprimirLinea(Interpreter interpreter, List<RunValue> args)
        {
            var arg = args.FirstOrDefault();
            Console.WriteLine(arg?.to_string());

            return new NullRunValue();
        }
        public RunValue imprimir(Interpreter interpreter, List<RunValue> args)
        {
            var arg = args.FirstOrDefault();
            Console.Write(arg?.to_string());

            return new NullRunValue();
        }
        public RunValue range(Interpreter interpreter, List<RunValue> args)
        {
            if (args.Count < 1 || args.Count > 2) throw new ArgumentException(string.Format("The {0} function expects {1} arguments, received {2}.", nameof(range), 2, args.Count));
            var arg = args.FirstOrDefault();
            var secondArg = args.LastOrDefault();
            if (arg is NumberRunValue numberArg)
            {
                var intStartArg = 0;
                var intEndArg = 0;
                if (secondArg is NumberRunValue second && second != null)
                {
                    intStartArg = Convert.ToInt32(numberArg.Value);
                    intEndArg = Convert.ToInt32(second.Value);
                }
                else
                {
                    intStartArg = 0;
                    intEndArg = Convert.ToInt32(numberArg.Value);
                }
                var listInt = Enumerable.Range(intStartArg, intEndArg)
                    .Select(x => new NumberRunValue(x) as RunValue)
                    .ToList();

                return new ListRunValue(listInt);
            }

            throw new NotImplementedException();


        }
        public RunValue receive_correct_type(Interpreter interpreter, List<RunValue> args)
        {
            if (!args.Any() || args.Count > 1)
            {
                throw new ArgumentException(string.Format("Function {0} expects {1} arguments, received {2}.", "type", 1, args.Count));
            }
            var arg = args.FirstOrDefault();
            return new StringRunValue(arg?.typestring() ?? string.Empty);
        }
        /// <summary>
        /// uso: load "../miotromodulo.top"
        /// </summary>
        /// <param name="interpreter"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <exception cref="InterpreterExceptions.UnitCompilationException"></exception>
        /// <exception cref="InterpreterExceptions.LoadModuleException"></exception>
        public RunValue load(Interpreter interpreter, List<RunValue> args)
        {
            arity("load", 1, args);

            var libraryPath = args?.FirstOrDefault()?.to_string();
            var interpreterDir = Directory.GetParent(interpreter.FilePath);

            if (libraryPath.StartsWith("."))
            {
                var modulePath = interpreterDir.FullName;
                if (libraryPath.EndsWith(".top"))
                {
                    modulePath = Path.Combine(modulePath, libraryPath);
                }
                else
                {
                    modulePath = Path.Combine(modulePath, string.Format("{0}{1}", libraryPath, ".top"));

                }

                var contents = File.ReadAllText(modulePath);
                var lexer = new LexicalAnalyzer(contents);
                var parser = new Parser(lexer);
                var program = parser.Parse();

                var execution = interpreter.Execute(program);
                if (execution) return new NullRunValue();

                throw new InterpreterExceptions.UnitCompilationException("could not compile unit source");


            }

            throw new InterpreterExceptions.LoadModuleException("could not find module");

        }

    }
}
