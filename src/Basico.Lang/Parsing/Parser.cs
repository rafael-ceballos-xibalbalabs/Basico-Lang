using Basico.Ast;
using Basico.Lexing;

namespace Basico.Parsing
{
    public class Parser
    {
        public LexicalAnalyzer _Lexer { get; set; }
        private Token _CurrentToken;
        private Token _NextToken;
        public Parser(LexicalAnalyzer lexer)
        {
            _Lexer = lexer;
            _CurrentToken = new Token(TokenKind.EOFToken, "\0");
            _NextToken = new Token(TokenKind.EOFToken, "\0");

            Next();
        }

        private void Next()
        {
            _CurrentToken = _NextToken;
            _NextToken = _Lexer.Lex();
        }
        private Statement ParseStatement()
        {
            Statement statement = default;

            if (_CurrentToken.kind != TokenKind.EOFToken && _CurrentToken.kind != TokenKind.EndLine)
            {
                switch (_CurrentToken.kind)
                {
                    case TokenKind.Var:
                        statement = ParseVarDeclaration();
                        break;

                    case TokenKind.For:
                        statement = ParseFor();
                        break;

                    case TokenKind.ForEach:
                        statement = ParseForEach();
                        break;

                    case TokenKind.While:
                        statement = ParseWhile();
                        break;

                    case TokenKind.DoWhile:
                        statement = ParseDoWhile();
                        break;

                    case TokenKind.If:
                        statement = ParseIf();
                        break;

                    case TokenKind.Function:
                        statement = ParseFunction(true);
                        break;
                    case TokenKind.Struct:
                        statement = ParseStruct();
                        break;

                    default:
                        var otherExpression = ParseExpression(Precedence.Lowest);
                        statement = new ExpressionStatement(otherExpression);
                        break;
                }
            }
            return statement;
        }

        private Statement ParseStruct()
        {
            ExpectTokenAndRead(TokenKind.Struct);
            var name = ExpectIdentifierAndRead()?.literal;
            ExpectTokenAndRead(TokenKind.LeftCurlyBrace);
            //ahora se recolectan los parámetros
            var fields = new List<Parameter>();
            while (_CurrentToken.kind != TokenKind.EOFToken && _CurrentToken.kind != TokenKind.RightCurlyBrace)
            {
                if (_CurrentToken.kind == TokenKind.Comma) ExpectTokenAndRead(TokenKind.Comma);
                var field = ExpectIdentifierAndRead()?.literal.ToString() ?? string.Empty;
                fields.Add(new Parameter(field));
            }

            ExpectTokenAndRead(TokenKind.RightCurlyBrace);
            return new StructDeclarationStatement(name?.ToString() ?? string.Empty, fields);
        }

        public AstProgram Parse()
        {
            var program = new AstProgram();
            var statements = new List<Statement>();

            Next(); // completa el priming que deja el constructor a medias (_CurrentToken todavía en el placeholder EOF)

            while (_CurrentToken.kind != TokenKind.EOFToken)
            {
                var stmt = ParseStatement();
                if (stmt is null) break;
                statements.Add(stmt);
            }
            program.Statements = statements;

            return program;
        }

        private Token ExpectTokenAndRead(TokenKind token)
        {
            var result = ExpectTokenAndThrows(token);
            Next();

            return result;
        }
        private Token ExpectToken(TokenKind token)
        {
            var result = ExpectTokenAndThrows(token);

            //Next();
            //var t = _CurrentToken;

            return result;
        }
        private Token ExpectIdentifierAndRead()
        {
            return ExpectTokenAndRead(TokenKind.Identifier);
            //return ExpectToken(TokenKind.Identifier);
        }
        private Token ExpectIdentifier()
        {
            return ExpectToken(TokenKind.Identifier);
        }

        private Token ExpectTokenAndThrows(TokenKind token)
        {
            if (_CurrentToken.kind == token) return _CurrentToken;
            throw new UnexpectedTokenException(_CurrentToken.literal.ToString());
        }

        private Statement ParseFunction(bool hasIdentifier)
        {
            ExpectTokenAndRead(TokenKind.Function);
            var name = hasIdentifier ? ExpectIdentifierAndRead()?.literal : "<Closure>";
            ExpectTokenAndRead(TokenKind.LeftParen);
            //ahora se recolectan los parámetros
            var parameters = new List<Parameter>();
            while (_CurrentToken.kind != TokenKind.EOFToken && _CurrentToken.kind != TokenKind.RightParen)
            {
                if (_CurrentToken.kind == TokenKind.Comma) ExpectTokenAndRead(TokenKind.Comma);
                var param = ExpectIdentifierAndRead()?.literal.ToString();
                parameters.Add(new Parameter(param));
            }

            ExpectTokenAndRead(TokenKind.RightParen);
            //ExpectToken(TokenKind.RightParen);

            var body = ParseBlock();

            return new FunctionDeclarationStatement(name.ToString(), parameters, body);
        }

        private Statement ParseIf()
        {
            ExpectTokenAndRead(TokenKind.If);
            var condition = ParseExpression(Precedence.Statement);
            var then = ParseBlock();
            var otherwise = new List<Statement>();
            if (_CurrentToken.kind == TokenKind.Else)
            {
                ExpectTokenAndRead(TokenKind.Else);
                otherwise = ParseBlock();
            }
            else
            {
                otherwise = null;
            }
            return new IfStatement(condition, then, otherwise);

        }
        private List<Statement> ParseBlock()
        {
            ExpectTokenAndRead(TokenKind.LeftCurlyBrace);
            var block = new List<Statement>();

            while (_CurrentToken.kind != TokenKind.RightCurlyBrace)
            {
                //block.Add(ParseStatement());
                block.Add(ParseStatement());

                if (_CurrentToken.kind == TokenKind.EndLine)
                {
                    ExpectTokenAndRead(TokenKind.EndLine);
                    continue;
                }

            }
            ExpectTokenAndRead(TokenKind.RightCurlyBrace);
            return block;
        }

        private Statement ParseWhile()
        {
            ExpectTokenAndRead(TokenKind.While);
            var condition = ParseExpression(Precedence.Statement);
            var block = ParseBlock();

            return new WhileStatement(condition, block);
        }

        private Statement ParseDoWhile()
        {
            ExpectTokenAndRead(TokenKind.DoWhile);
            var condition = ParseExpression(Precedence.Statement);
            var block = ParseBlock();

            return new DoWhileStatement(condition, block);
        }

        private Statement ParseForEach()
        {
            ExpectTokenAndRead(TokenKind.ForEach);
            var first = ExpectIdentifierAndRead()?.literal?.ToString();

            string value;
            string? index = null;
            if (_CurrentToken.kind == TokenKind.Comma)
            {
                ExpectTokenAndRead(TokenKind.Comma);
                index = first;
                value = ExpectIdentifierAndRead()?.literal?.ToString();
            }
            else
            {
                value = first;
            }

            ExpectTokenAndRead(TokenKind.In);
            var iterable = ParseExpression(Precedence.Statement);
            var block = ParseBlock();

            return new ForEachStatement(iterable, value, index, block);
        }

        private Statement ParseFor()
        {
            ExpectTokenAndRead(TokenKind.For);
            var init = ParseStatement();
            ExpectTokenAndRead(TokenKind.EndLine);
            var condition = ParseExpression(Precedence.Statement);
            ExpectTokenAndRead(TokenKind.EndLine);
            var postExpression = ParseExpression(Precedence.Statement);
            var post = new ExpressionStatement(postExpression);
            var block = ParseBlock();

            return new ForStatement(init, condition, post, block);
        }

        private Statement ParseVarDeclaration()
        {
            ExpectTokenAndRead(TokenKind.Var);
            var name = ExpectIdentifierAndRead().literal.ToString();
            StatementExpression initial = default;
            if (_CurrentToken.kind == TokenKind.Assign)
            {
                ExpectTokenAndRead(TokenKind.Assign);
                initial = ParseExpression(Precedence.Lowest);
            }
            else
            {
                initial = null;
            }
            var current = _CurrentToken;
            return new VarStatement(name, initial);
        }
        private StatementExpression ParseExpression(Precedence precedence)
        {
            StatementExpression left = default;
            switch (_CurrentToken.kind)
            {
                case TokenKind.StringLiteral:
                    var literal = _CurrentToken?.literal?.ToString();
                    ExpectTokenAndRead(TokenKind.StringLiteral);
                    left = new StringExpression(literal);
                    break;

                case TokenKind.Null:
                    ExpectTokenAndRead(TokenKind.Null);
                    left = new NullExpression();
                    break;

                case TokenKind.NumericLiteral:
                    var number = (double)_CurrentToken.literal;
                    ExpectTokenAndRead(TokenKind.NumericLiteral);
                    left = new NumericExpression(number);
                    break;

                case TokenKind.True:
                    ExpectTokenAndRead(TokenKind.True);
                    left = new BooleanExpression(true);
                    break;
                case TokenKind.False:
                    ExpectTokenAndRead(TokenKind.False);
                    left = new BooleanExpression(false);
                    break;

                case TokenKind.Identifier:
                    var idToken = ExpectIdentifierAndRead();
                    //left = new IdentifierExpression(_CurrentToken.literal.ToString());
                    left = new IdentifierExpression(idToken.literal.ToString());
                    break;

                case TokenKind.Function:
                    var functionStatement = ParseFunction(false) as FunctionDeclarationStatement;
                    left = new ClosureExpression(functionStatement?.Parameters, functionStatement?.Body);
                    break;

                case TokenKind.Minus:
                    ExpectTokenAndRead(TokenKind.Minus);
                    left = new PrefixExpression(Op.Subtract, ParseExpression(Precedence.Prefix));
                    break;

                case TokenKind.Not:
                    ExpectTokenAndRead(TokenKind.Not);
                    left = new PrefixExpression(Op.NotEquals, ParseExpression(Precedence.Prefix));
                    break;

                case TokenKind.OpenSquareBracket: //[1,2,3]
                    ExpectTokenAndRead(TokenKind.OpenSquareBracket);
                    var items = new List<StatementExpression>();
                    while (_CurrentToken.kind != TokenKind.CloseSquareBracket)
                    {
                        items.Add(ParseExpression(Precedence.Lowest));
                        if (_CurrentToken.kind == TokenKind.Comma)
                        {
                            ExpectTokenAndRead(TokenKind.Comma);
                        }
                    }
                    ExpectTokenAndRead(TokenKind.CloseSquareBracket);

                    left = new ListExpression(items);
                    break;

                default:
                    throw new UnexpectedTokenException(_CurrentToken.literal.ToString());
                    //return null;

            }

            while (_CurrentToken.kind != TokenKind.EOFToken && _CurrentToken.kind != TokenKind.EndLine && (int)precedence < (int)ConvertTokenKindToPrecedence(_CurrentToken.kind))
            {
                StatementExpression expP = ParsePostfixExpression(left);
                StatementExpression expIn = ParseInfixExpression(left);

                if (expP != null)
                {
                    left = expP;
                }
                else if (expIn != null)
                {
                    left = expIn;
                }
                else
                {
                    break;
                }

            }
            return left;
        }
        private StatementExpression ParsePostfixExpression(StatementExpression left)
        {
            //StatementExpression left = default;
            switch (_CurrentToken.kind)
            {
                case TokenKind.Dot:
                    ExpectTokenAndRead(TokenKind.Dot);
                    var field = ExpectIdentifierAndRead()?.literal.ToString();
                    return new GetExpression(left, field);

                case TokenKind.OpenSquareBracket:
                    ExpectTokenAndRead(TokenKind.OpenSquareBracket);
                    var index = _CurrentToken.kind == TokenKind.CloseSquareBracket ? null : ParseExpression(Precedence.Lowest);
                    ExpectTokenAndRead(TokenKind.CloseSquareBracket);
                    return new ListIndexExpression(left, index);


                case TokenKind.LeftCurlyBrace:
                    ExpectTokenAndRead(TokenKind.LeftCurlyBrace);
                    var fields = new Dictionary<string, StatementExpression>();
                    while (_CurrentToken.kind != TokenKind.RightCurlyBrace)
                    {
                        var fieldKey = ExpectIdentifierAndRead().ToString();
                        StatementExpression value = default;
                        if (_CurrentToken.kind == TokenKind.Colon)
                        {
                            ExpectTokenAndRead(TokenKind.Colon);
                            value = ParseExpression(Precedence.Lowest);
                        }
                        else
                        {
                            value = new IdentifierExpression(fieldKey);
                        }

                        fields.Add(fieldKey, value);
                        if (_CurrentToken.kind == TokenKind.Comma) Next();

                    }
                    ExpectTokenAndRead(TokenKind.RightCurlyBrace);
                    return new StructExpression(left, fields);


                case TokenKind.LeftParen:
                    ExpectTokenAndRead(TokenKind.LeftParen);
                    var args = new List<StatementExpression>();
                    while (_CurrentToken.kind != TokenKind.RightParen)
                    {
                        args.Add(ParseExpression(Precedence.Lowest));
                        if (_CurrentToken.kind == TokenKind.Comma) Next();
                    }
                    ExpectTokenAndRead(TokenKind.RightParen);
                    return new CallExpression(left, args);


                default:
                    return null;
            }
        }
        private StatementExpression ParseInfixExpression(StatementExpression left)
        {
            switch (_CurrentToken.kind)
            {
                case TokenKind.Plus:
                case TokenKind.Minus:
                case TokenKind.Multiply:
                case TokenKind.Divide:
                case TokenKind.Equality:
                case TokenKind.NotEqual:
                case TokenKind.LessThanOrEquals:
                case TokenKind.LessThan:
                case TokenKind.GreaterThan:
                case TokenKind.GreaterThanOrEquals:
                case TokenKind.And:
                case TokenKind.Or:

                    var token = _CurrentToken;
                    Next();
                    var right = ParseExpression(ConvertTokenKindToPrecedence(token.kind));

                    return new InfixExpression(left, ConvertTokenKindToOperand(token?.kind), right);

                case TokenKind.Assign:
                    Next();
                    // Precedence.Statement (no Precedence.Lowest) evita que un "{" que venga inmediatamente
                    // después (p. ej. el bloque de "repetir_hasta ...; i = i + 1 { ... }") se malinterprete como
                    // un literal de struct sobre el lado derecho de la asignación.
                    var rght = ParseExpression(Precedence.Statement);
                    return new AssignmentExpression(left, rght);

                default:
                    return null;
            }
        }

        private Op ConvertTokenKindToOperand(TokenKind? kind)
        {
            switch (kind)
            {
                case TokenKind.Plus: return Op.Add;
                case TokenKind.Minus: return Op.Subtract;
                case TokenKind.Multiply: return Op.Multiply;
                case TokenKind.Divide: return Op.Divide;
                case TokenKind.Not: return Op.NotEquals;
                case TokenKind.Modulo: return Op.Modulo;
                case TokenKind.Equality: return Op.Equals;
                case TokenKind.Assign: return Op.Assign;
                case TokenKind.LessThan: return Op.LessThan;
                case TokenKind.GreaterThan: return Op.GreaterThan;
                case TokenKind.LessThanOrEquals: return Op.LessThanOrEquals;
                case TokenKind.GreaterThanOrEquals: return Op.GreaterThanOrEquals;
                case TokenKind.And: return Op.And;
                case TokenKind.Or: return Op.Or;
                case TokenKind.Pow: return Op.Pow;

                default: throw new NotImplementedException();
            }
        }

        private Precedence ConvertTokenKindToPrecedence(TokenKind kind)
        {
            switch (kind)
            {
                case TokenKind.Multiply:
                case TokenKind.Divide:
                    return Precedence.Product;

                case TokenKind.Plus:
                case TokenKind.Minus:
                    return Precedence.Sum;

                case TokenKind.LeftParen:
                case TokenKind.Dot:
                case TokenKind.OpenSquareBracket:
                    return Precedence.Call;

                case TokenKind.LessThan:
                case TokenKind.GreaterThan:
                case TokenKind.LessThanOrEquals:
                case TokenKind.GreaterThanOrEquals:
                    return Precedence.LessThanGreaterThan;


                case TokenKind.Equality:
                case TokenKind.NotEqual:
                    return Precedence.Equals;

                case TokenKind.And:
                case TokenKind.Or:
                    return Precedence.AndOr;

                case TokenKind.Assign:
                    return Precedence.Assign;

                case TokenKind.LeftCurlyBrace:
                    return Precedence.Statement;

                case TokenKind.Pow:
                    return Precedence.Pow;

                default:
                    return Precedence.Lowest;
            }


        }
    }
}
