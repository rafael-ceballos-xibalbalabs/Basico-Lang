using Basico.Lexing;
using Basico.Parsing;
using Basico.Runtime;

namespace Basico
{
    internal class Program
    {
        public static string CurrentDirectory { get; } = System.IO.Directory.GetCurrentDirectory();
        static void Main(string[] args)
        {
            try
            {
                InterpreteArgs(args);
            }
            catch (System.Exception ex)
            {
#if DEBUG
                PrintError($"Excepción no controlada: {ex.Message}\n{ex.StackTrace}");
#endif
                PrintError(ex.Message);
                System.Environment.Exit(1);
            }
            finally
            {
                // Asegura que la consola vuelva a los colores por defecto antes de salir
                System.Console.ResetColor();
            }
        }
        private static void PrintLogo()
        {
            // Imprime el logo en verde y luego reinicia el color
            System.Console.ForegroundColor = System.ConsoleColor.Green;
            System.Console.BackgroundColor = System.ConsoleColor.Black;
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            System.Console.WriteLine(@"···············································································");
            System.Console.WriteLine(@":                                                                             :");
            System.Console.WriteLine(@": ██████╗ █████╗█████████╗██████╗██████╗     ██╗     █████╗███╗   ██╗██████╗  :");
            System.Console.WriteLine(@": ██╔══████╔══████╔════████╔════██╔═══██╗    ██║    ██╔══██████╗  ████╔════╝  :");
            System.Console.WriteLine(@": ██████╔██████████████████║    ██║   ██║    ██║    █████████╔██╗ ████║  ███╗ :");
            System.Console.WriteLine(@": ██╔══████╔══██╚════██████║    ██║   ██║    ██║    ██╔══████║╚██╗████║   ██║ :");
            System.Console.WriteLine(@": ██████╔██║  ███████████╚██████╚██████╔╝    █████████║  ████║ ╚████╚██████╔╝ :");
            System.Console.WriteLine(@": ╚═════╝╚═╝  ╚═╚══════╚═╝╚═════╝╚═════╝     ╚══════╚═╝  ╚═╚═╝  ╚═══╝╚═════╝  :");
            System.Console.WriteLine(@":                                                                             :");
            System.Console.WriteLine($"·······························································v{version}········");
            // Reinicia los colores
            System.Console.ResetColor();
        }
        private static void PrintHelp()
        {
            System.Console.WriteLine("Uso: Basico [parametros]");
            System.Console.WriteLine("Options:");
            System.Console.WriteLine("  -nologo     No mostrar el logo.");
            System.Console.WriteLine("  -help       Mostrar el texto de ayuda.");
            System.Console.WriteLine("  -v          Mostrar el número de versión.");
        }
        private static void PrintError(string message)
        {
            System.Console.ForegroundColor = System.ConsoleColor.Red;
            System.Console.WriteLine($"Error: {message}");
            System.Console.ResetColor();
        }
        private static void PrintVersion()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            System.Console.WriteLine($"Versión: {version}");
        }
        private static void InterpreteArgs(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                RunDefault(args);
                return;
            }
            foreach (var arg in args)
            {
                switch (arg.ToLower())
                {
                    case "-nologo":
                        // Sin logo, se maneja en Main
                        args = args.Where(a => a.ToLower() != "-nologo").ToArray();
                        RunDefault([arg]);
                        break;
                    case "-help":
                        PrintHelp();
                        break;
                    case "-v":
                        PrintVersion();
                        break;
                    default:
                        RunDefault([arg]);
                        break;
                }
            }
        }
        private static void RunDefault(string[] args)
        {
            PrintLogo();
            PrintHelp();
            RunBasicoFiles(args);
        }
        private static void RunBasicoFiles(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                // Si no hay argumentos, ejecuta el primer archivo .basico encontrado
                var firstFile = GetFirstBasicoFile();
                RunBasicoFile(firstFile);
            }
            else
            {
                foreach (var filePath in args)
                {
                    RunBasicoFile(filePath);
                }
            }
        }
        private static void RunBasicoFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }
            if (!System.IO.File.Exists(filePath))
            {
                throw new System.IO.FileNotFoundException($"El archivo '{filePath}' no existe.");
            }
            // Carga el código fuente
            System.Console.WriteLine($"Cargando el archivo: {filePath}");
            var sourceCodeLoader = new SourceCodeLoader(filePath);
            var sourceCode = sourceCodeLoader.ReadSourceCode();
            // Analiza léxicamente el código fuente
            var lexicalAnalyzer = new LexicalAnalyzer(sourceCode);
            var parser = new Parser(lexicalAnalyzer);
            var program = parser.Parse();
            
            var interpreter = new Interpreter(program, filePath);

            interpreter.Run();
        }
        private static string GetFirstBasicoFile()
        {
            var files = GetBasicoFiles();
            if (files.Length == 0)
            {
                Console.WriteLine("No se encontraron archivos .basico en el directorio actual.");
                return string.Empty;
            }
            return files[0];
        }
        private static string[] GetBasicoFiles()
        {
            return System.IO.Directory.GetFiles(CurrentDirectory, "*.basico")
                .OrderBy(file => file)
                .ToArray();
        }
    }
}