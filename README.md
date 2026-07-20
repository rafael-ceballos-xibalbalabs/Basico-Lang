<div align="center">

# 📘 Básico Lang

**Un lenguaje de programación interpretado con sintaxis 100% en español, diseñado para enseñar los fundamentos de la programación sin la barrera del idioma.**

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Lenguaje](https://img.shields.io/badge/lenguaje-C%23-239120?style=flat-square&logo=csharp)](https://learn.microsoft.com/dotnet/csharp/)
[![Build Status](https://img.shields.io/badge/build-en%20desarrollo-yellow?style=flat-square)](#)
[![Estado](https://img.shields.io/badge/estado-en%20desarrollo-orange?style=flat-square)](#)
[![AOT Ready](https://img.shields.io/badge/Native%20AOT-compatible-blue?style=flat-square)](https://learn.microsoft.com/dotnet/core/deploying/native-aot)
[![Licencia](https://img.shields.io/badge/licencia-MIT-green?style=flat-square)](#-licencia)
[![PRs Welcome](https://img.shields.io/badge/PRs-bienvenidos-brightgreen?style=flat-square)](#-contribución)

</div>

---

## 📝 Overview

**Básico Lang** es un intérprete *tree-walking* construido en C# / .NET 8 para un lenguaje de programación dinámicamente tipado cuyas palabras clave están escritas íntegramente en español (`variable`, `si`/`sino`, `funcion`, `repetir`, `devolver`, `verdadero`/`falso`, `nulo`...).

El objetivo del proyecto es eliminar la fricción que supone aprender a programar en inglés cuando ese no es tu idioma nativo, ofreciendo una sintaxis simple, cercana y legible para quienes están dando sus primeros pasos en programación — sin sacrificar conceptos reales como funciones, closures, structs, listas y tipado dinámico.

Internamente, el proyecto es también un caso de estudio interesante para cualquier ingeniero interesado en cómo se construye un lenguaje desde cero: lexer manual, parser Pratt (precedence-climbing) y un intérprete recursivo, todo compilable con **Native AOT**.

---

## 🚀 Características Principales

- 🇪🇸 **Sintaxis totalmente en español** — palabras clave como `variable`, `funcion`, `si`/`sino`, `devolver`, `verdadero`/`falso`, `nulo`, `en`, pensadas para bajar la barrera de entrada a la programación.
- 🧩 **Pipeline de intérprete clásico y didáctico** — `SourceCodeLoader → LexicalAnalyzer → Parser → Interpreter`, ideal para estudiar o extender cómo funciona un lenguaje internamente.
- 🌲 **Parser Pratt (precedence-climbing)** — manejo correcto de precedencia de operadores aritméticos, lógicos y de comparación mediante `ParseExpression`/`ConvertTokenKindToPrecedence`.
- 🧠 **Tipado dinámico con valores en tiempo de ejecución explícitos** — `RunValue` y sus variantes (`NumberRunValue`, `StringRunValue`, `BooleanRunValue`, `ListRunValue`, `FunctionRunValue`, `StructRunValue`, entre otras) modelan cada tipo del lenguaje de forma explícita.
- 🏗️ **Structs y funciones de primera clase** — soporte para definición de `struct`, literales de struct (`Persona { nombre: "Ana" }`), funciones con clausuras (`closures`) y funciones nativas.
- 📚 **Biblioteca estándar embebida** — funciones globales (`imprimir`, `imprimirLinea`, `type`, `load`) y métodos nativos en español sobre `String`, `Number` y `List` (`.contiene()`, `.a_mayusculas()`, `.es_entero()`, `.transformar()`, `.filtrar()`, `.unir()`, etc.).
- 📦 **Sistema de módulos simple** — la función nativa `load` permite interpretar otro archivo (`.top`) dentro de la misma instancia del intérprete, habilitando reutilización de código entre scripts.
- ⚡ **Native AOT / Trimming friendly** — el proyecto se compila con `PublishAot=true` e `InvariantGlobalization=true`, apuntando a binarios nativos pequeños y de arranque rápido, sin dependencia del runtime de .NET instalado en la máquina destino.
- 🖥️ **CLI simple** — ejecuta un script pasando su ruta, o deja que el intérprete descubra automáticamente el primer archivo `*.basico` del directorio actual.

---

## 🛠️ Requisitos Previos

Antes de compilar o ejecutar el proyecto necesitas tener instalado:

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) o superior.
- Un editor de código con soporte para C# (recomendado: [Visual Studio 2022](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/) con la extensión de C# Dev Kit).
- (Opcional) Herramientas de compilación nativa de tu sistema operativo si planeas publicar con `PublishAot=true` (por ejemplo, el workload de C++ en Windows, o `clang`/`zlib1g-dev` en Linux).

---

## 📥 Instalación

Clona el repositorio y restaura las dependencias del proyecto principal (`src/Basico`):

```bash
# 1. Clona el repositorio
git clone https://github.com/xibalbalabs/Basico-Lang.git
cd Basico-Lang/src/Basico

# 2. Restaura dependencias y compila
dotnet build Basico.csproj
```

Para generar un binario nativo autocontenido (Native AOT):

```bash
dotnet publish Basico.csproj -c Release -r win-x64 --self-contained
# Sustituye win-x64 por linux-x64 / osx-x64 / osx-arm64 según tu plataforma
```

---

## ▶️ Uso

### Desde la línea de comandos

Crea un archivo `hola.basico`:

```
funcion saludar(nombre) {
    devolver "Hola, " + nombre + "!"
}

variable mensaje = saludar("Mundo")
imprimirLinea(mensaje)

si mensaje != nulo {
    imprimirLinea("El programa terminó correctamente")
} sino {
    imprimirLinea("Algo salió mal")
}
```

Y ejecútalo con el intérprete:

```bash
dotnet run --project Basico.csproj -- hola.basico
```

Si no indicas ningún archivo, Básico Lang buscará y ejecutará automáticamente el primer `*.basico` que encuentre en el directorio actual:

```bash
dotnet run --project Basico.csproj
```

Flags disponibles: `-nologo` (omite el logo), `-help` (muestra la ayuda), `-v` (muestra la versión).

### Embebiendo el pipeline del intérprete en C#

El motor completo del lenguaje vive en el proyecto **`Basico.Lang`** (`src/Basico.Lang/Basico.Lang.csproj`), una
class library independiente de la CLI. `Program.cs` (en `src/Basico/`, el proyecto que produce el `.exe`) es solo
uno de sus consumidores — cualquier otro ejecutable .NET puede referenciar `Basico.Lang.csproj`/`Basico.Lang.dll` y
reutilizar el mismo pipeline `SourceCodeLoader → LexicalAnalyzer → Parser → Interpreter`:

```csharp
using Basico;
using Basico.Ast;
using Basico.Lexing;
using Basico.Parsing;
using Basico.Runtime;

// 1. Carga el código fuente desde disco
var sourceCodeLoader = new SourceCodeLoader("hola.basico");
var sourceCode = sourceCodeLoader.ReadSourceCode();

// 2. Análisis léxico + parsing a un AST
var lexicalAnalyzer = new LexicalAnalyzer(sourceCode);
var parser = new Parser(lexicalAnalyzer);
AstProgram program = parser.Parse();

// 3. Interpretación del AST resultante
var interpreter = new Interpreter(program, "hola.basico");
interpreter.Interprete();
```

`SourceCodeLoader`, `LexicalAnalyzer` y `Parser` son `public` específicamente para habilitar este flujo desde fuera
del proyecto (antes eran `internal`). Dentro de `LexicalAnalyzer` solo el constructor es parte de la superficie
pública pensada para consumidores externos — `Lex()`/`Next()`/`ConsumeWhiteSpace()` (y los tipos `Token`/`TokenKind`
que manejan) siguen siendo `internal`, ya que solo los usa `Parser` internamente.

---

## 🏛️ Arquitectura / Estructura

```
src/
├── Basico.Lang/                   # El motor del lenguaje (class library, reutilizable)
│   ├── SourceCodeLoader.cs            # Carga el archivo .basico desde disco
│   ├── Lexing/                        # Etapa 2: texto → tokens
│   │   ├── LexicalAnalyzer.cs             # Lexer manual, produce un Token por llamada a Lex()
│   │   ├── Token.cs, TokenKind.cs
│   ├── Parsing/                       # Etapa 3: tokens → AST
│   │   ├── Parser.cs                      # Parser Pratt / precedence-climbing → AstProgram
│   │   ├── Precedence.cs, SyntaxException.cs, UnexpectedTokenException.cs
│   ├── Ast/                           # Nodos del AST que produce el Parser
│   │   ├── AstProgram.cs
│   │   ├── Statements/                    # VarStatement, IfStatement, WhileStatement, ForStatement, ...
│   │   └── Expressions/                   # InfixExpression, CallExpression, ListExpression, ...
│   └── Runtime/                       # Etapa 4: AST → efectos
│       ├── Interpreter.cs                 # Recorre el AST (RunStatement / RunExpression)
│       ├── RunValue.cs                    # Modelo de valores en tiempo de ejecución
│       ├── InterpreterEnvironment.cs, InterpreterExceptions.cs, Extensions.cs
│       └── Natives/                       # Funciones y métodos nativos (biblioteca estándar)
│           ├── Common.cs                      # imprimir, imprimirLinea, type, load
│           ├── StringObject.cs                # Métodos nativos sobre strings
│           ├── NumberObject.cs                # Métodos nativos sobre números
│           └── ListObject.cs                  # Métodos nativos sobre listas
├── Basico/                        # CLI runner (produce el .exe; referencia Basico.Lang)
│   └── Program.cs                     # Entry point: argumentos, logo/ayuda, detección de .basico
└── ConsoleApp1/                   # Scaffold de consola sin relación con el intérprete (ignorar)
```

El código fuente se procesa en cuatro etapas encadenadas en `Program.RunBasicoFile`:

```
SourceCodeLoader → LexicalAnalyzer (Lex) → Parser (Parse) → Interpreter (Interprete)
```

El intérprete mantiene dos ámbitos de resolución de identificadores independientes: `Globals` (funciones, structs y builtins nativos) y `Environment`, un único ámbito **plano** (no anidado) para variables, parámetros de función y bindings de bucles — las funciones cierran sobre el objeto `Environment` que estaba activo en su definición, actuando como closures.

---

## 🤝 Contribución

¡Toda contribución es bienvenida! Este proyecto está en desarrollo activo y hay funcionalidades incompletas (por ejemplo, el bucle `repetir` aún no está totalmente implementado en el parser).

1. Haz un **fork** del repositorio.
2. Crea una rama descriptiva para tu cambio:
   ```bash
   git checkout -b feature/nombre-de-tu-mejora
   ```
3. Realiza tus cambios y verifica que el proyecto compila:
   ```bash
   dotnet build Basico.csproj
   ```
4. Haz commit de tus cambios con un mensaje claro:
   ```bash
   git commit -m "Agrega soporte para X"
   ```
5. Sube tu rama y abre un **Pull Request** describiendo el cambio, su motivación y cómo probarlo.

Si vas a añadir una funcionalidad nueva al lenguaje (una palabra clave, un operador, un método nativo), sigue el estilo existente del código: por ejemplo, los métodos nativos de `String`/`Number`/`List` se añaden como un nuevo `case` en el switch `Get(name)` correspondiente, y los operadores nuevos como un nuevo *pattern match* en `Interpreter.RunExpression`.

---

## 📄 Licencia

Este proyecto se distribuye bajo la licencia **MIT**. Consulta el archivo [`LICENSE`](LICENSE) para más información.

> Nota: el repositorio aún no incluye un archivo `LICENSE` formal — se recomienda añadirlo antes de aceptar contribuciones externas.
