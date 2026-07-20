using System.Text;

namespace Basico.Lexing
{
    public class LexicalAnalyzer
    {
        private IList<Token> _tokens;
        public string _Source { get; set; }
        private char _CurrentChar;
        private char _NextChar;
        private int _Position;
        public LexicalAnalyzer()
        {
            _tokens = new List<Token>();
        }

        public LexicalAnalyzer(string input)
        {
            _Source = input ?? throw new ArgumentNullException(nameof(input));
            _Position = -2;

            _CurrentChar = '\0';
            _NextChar = '\0';

            Next();
        }
        internal void ConsumeWhiteSpace()
        {
            while (_CurrentChar != '\0' && char.IsWhiteSpace(_CurrentChar))
            {
                Next();
            }
        }
        internal void Next()
        {
            _Position++;
            _CurrentChar = _NextChar;

            if (_Position <= (_Source.Length - 2))
            {
                _NextChar = _Source[_Position + 1];
            }
            else
            {
                _NextChar = '\0';
            }
        }

        internal Token Lex()
        {
            var token = new Token(TokenKind.EOFToken, "\0");
            Next();
            ConsumeWhiteSpace();
            var currentChar = _CurrentChar;
            var charString = currentChar.ToString();

            if (currentChar == ';')
            {
                return new Token(TokenKind.EndLine, charString);
            }

            if (currentChar == '\0')
            {
                token = new Token(TokenKind.EOFToken, charString);
                return token;
            }
            else if (currentChar == '(')
            {
                return new Token(TokenKind.LeftParen, charString);
            }
            else if (currentChar == ')')
            {
                return new Token(TokenKind.RightParen, charString);
            }
            else if (char.IsLetter(currentChar)) //posible inicio de una palabra reservada - var
            {
                var prevToken = MakeIdentifierLiteral();

                if (IsReservedKeyword(prevToken?.literal?.ToString() ?? string.Empty, out var kind))
                {
                    //esto es casi seguro la palabra clave var...
                    //return MakeReservedKeyword();
                    return new Token(kind, prevToken?.literal?.ToString() ?? string.Empty);
                }

                //probablemente un identificador
                return new Token(TokenKind.Identifier, prevToken?.literal?.ToString() ?? string.Empty);

            }
            else if (currentChar == '=')
            {
                var sb = new StringBuilder();
                sb.Append(currentChar);
                if (_NextChar == '=')
                {
                    sb.Append(_NextChar);
                    return new Token(TokenKind.Equality, sb.ToString());
                }
                return new Token(TokenKind.Assign, charString);
            }
            else if (currentChar == '<')
            {
                var sb = new StringBuilder();
                sb.Append(currentChar);
                if (_NextChar == '=')
                {
                    sb.Append(_NextChar);
                    return new Token(TokenKind.LessThanOrEquals, sb.ToString());
                }
                return new Token(TokenKind.LessThan, charString);
            }
            else if (currentChar == '>')
            {
                var sb = new StringBuilder();
                sb.Append(currentChar);
                if (_NextChar == '=')
                {
                    sb.Append(_NextChar);
                    return new Token(TokenKind.GreaterThanOrEquals, sb.ToString());
                }
                return new Token(TokenKind.GreaterThan, charString);
            }
            else if (currentChar == '&')
            {
                var sb = new StringBuilder();
                sb.Append(currentChar);
                if (_NextChar == '&')
                {
                    sb.Append(_NextChar);
                    return new Token(TokenKind.And, sb.ToString());
                }

                return new Token(TokenKind.Ampersand, charString);

            }
            else if (currentChar == '\"')
            {
                return MakeStringLiteral();
            }
            else if (char.IsDigit(currentChar))
            {
                return MakeNumericLiteral();
            }
            else if (currentChar == '+')
            {
                return new Token(TokenKind.Plus, charString);
            }
            else if (currentChar == '-')
            {
                return new Token(TokenKind.Minus, charString);
            }
            else if (currentChar == '*')
            {
                return new Token(TokenKind.Multiply, charString);
            }
            else if (currentChar == '/')
            {
                return new Token(TokenKind.Divide, charString);
            }
            else if (currentChar == ':')
            {
                return new Token(TokenKind.Colon, charString);
            }

            else if (currentChar == ',')
            {
                return new Token(TokenKind.Comma, charString);
            }
            else if (currentChar == '{')
            {
                return new Token(TokenKind.LeftCurlyBrace, charString);
            }
            else if (currentChar == '}')
            {
                return new Token(TokenKind.RightCurlyBrace, charString);
            }

            else if (currentChar == '[')
            {
                return new Token(TokenKind.OpenSquareBracket, charString);
            }
            else if (currentChar == ']')
            {
                return new Token(TokenKind.CloseSquareBracket, charString);
            }
            else if (currentChar == '!')
            {
                var sb = new StringBuilder();
                sb.Append(currentChar);
                if (_NextChar == '=')
                {
                    sb.Append(_NextChar);
                    return new Token(TokenKind.NotEqual, sb.ToString());
                }
                return new Token(TokenKind.Not, charString);
            }
            else if (currentChar == '|')
            {
                var sb = new StringBuilder();
                sb.Append(currentChar);
                if (_NextChar == '|')
                {
                    sb.Append(_NextChar);
                    return new Token(TokenKind.Or, sb.ToString());
                }
                return new Token(TokenKind.Pipe, charString);
            }
            else if (currentChar == '%')
            {
                return new Token(TokenKind.Modulo, charString);
            }
            else if (currentChar == '.')
            {
                return new Token(TokenKind.Dot, charString);
            }

            return new Token(TokenKind.EOFToken, "\0");
        }
        private bool IsReservedKeyword(string keyword, out TokenKind kind)
        {
            switch (keyword.ToLowerInvariant())
            {
                case "variable":
                    kind = TokenKind.Var;
                    break;
                case "repetir":
                    kind = TokenKind.While;
                    break;
                case "repetir_mientras":
                    kind = TokenKind.DoWhile;
                    break;
                case "iterar":
                    kind = TokenKind.ForEach;
                    break;
                case "repetir_hasta":
                    kind = TokenKind.For;
                    break;

                case "verdadero":
                    kind = TokenKind.True;
                    break;
                case "falso":
                    kind = TokenKind.False;
                    break;
                case "nulo":
                    kind = TokenKind.Null;
                    break;
                case "funcion":
                    kind = TokenKind.Function;
                    break;

                case "si":
                    kind = TokenKind.If;
                    break;
                case "sino":
                    kind = TokenKind.Else;
                    break;
                case "en":
                    kind = TokenKind.In;
                    break;
                case "devolver":
                    kind = TokenKind.Return;
                    break;
                default:
                    kind = TokenKind.EOFToken;
                    break;
            }

            if (kind != TokenKind.EOFToken)
            {
                return true;
            }

            return false;
        }
        private Token MakeNumericLiteral()
        {
            var currentPos = _Position;
            var dots = 0;
            while (_CurrentChar != '\0' && "0123456789._".Contains(_NextChar))
            {
                if (_CurrentChar == '.')
                {
                    dots++;
                }
                Next();
            }
            if (dots > 1) throw new FormatException("formato numérico no válido.");
            var numberValue = _Source.Substring(currentPos, _Position - currentPos + 1);
            var cleaned = numberValue.Replace("_", "");
            return new Token(TokenKind.NumericLiteral, double.Parse(cleaned));
        }
        private Token MakeStringLiteral()
        {
            Next();
            var currentPos = _Position;
            while (_CurrentChar != '\0' && _CurrentChar != '\"')
            {
                Next();
            }

            var literal = _Source.Substring(currentPos, _Position - currentPos);

            return new Token(TokenKind.StringLiteral, literal);
        }
        private Token MakeIdentifierLiteral()
        {
            var currentPos = _Position;
            //Next();
            while (_CurrentChar != '\0' && _CurrentChar != ';' && (char.IsLetter(_NextChar) || _NextChar == '_'))
            {
                Next();
            }

            var literal = _Source.Substring(currentPos, _Position - currentPos + 1);

            return new Token(TokenKind.Identifier, literal);
        }
    }
}
