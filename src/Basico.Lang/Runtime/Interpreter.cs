using System.Text;
using Basico.Ast;
using static Basico.Runtime.RunValue;

namespace Basico.Runtime
{
    public class Interpreter : IInterpreter
    {
        private readonly Common _common = new Common();
        public Interpreter(AstProgram ast, string filePath)
        {
            Ast = ast;
            FilePath = filePath;

            Environment = new InterpreterEnvironment();
            Globals = new Dictionary<string, RunValue>();
            RegisterGlobalFunctions();
            //Console.WriteLine("AST Depth: {0}", ast.Statements.Count);
            //Console.WriteLine("Starting interpretation of {0} ...", filePath);
        }

        public AstProgram Ast { get; }
        public InterpreterEnvironment Environment { get; set; }
        public Dictionary<string, RunValue> Globals { get; set; }
        public string FilePath { get; }

        private void RegisterGlobalFunctions()
        {
            DefineGlobalFunction("imprimir", _common.imprimir);
            DefineGlobalFunction("imprimirLinea", _common.imprimirLinea);
            DefineGlobalFunction("type", _common.receive_correct_type);
            DefineGlobalFunction("load", _common.load);
        }

        public bool Run()
        {
            return Execute(Ast);
        }

        public RunValue? Call(RunValue callable, List<RunValue> arguments)
        {
            if (callable is NativeFunctionRunValue nativeFunction)
            {
                return nativeFunction.Callback(this, arguments);
            }
            else if (callable is NativeMethodRunValue nativeMethod)
            {
                var context = RunExpression(nativeMethod?.Context);
                return nativeMethod.Callback(this, context, arguments);

            }
            else if (callable is FunctionRunValue functionRunValue)
            {
                if (functionRunValue?.Parameters.FirstOrDefault() is null && functionRunValue?.Parameters.Count != arguments.Count)
                {
                    throw new InterpreterExceptions.TooFewArgumentsException("missing arguments.");
                }
                var oldEnv = this.Environment;
                var newEnv = functionRunValue?.Environment ?? new InterpreterEnvironment();
                if (functionRunValue?.Context != null && functionRunValue?.Parameters.FirstOrDefault() != null)
                {
                    var context = RunExpression(functionRunValue?.Context);
                    newEnv.Set("this", context);
                }

                var w = functionRunValue.Parameters.Where(x => x.Name != "this").Zip(arguments);
                foreach ((var Param, var value) in w)
                {
                    newEnv.Set(Param?.Name, value);
                }

                Environment = newEnv;
                RunValue returnValue = default;
                foreach (var stmt in functionRunValue?.Block)
                {
                    RunStatement(stmt);
                }
                Environment = oldEnv;
                return returnValue;
            }

            throw new NotImplementedException();
        }

        public bool Execute(AstProgram ast)
        {
            //TODO: crear un TryExecute que emita/devuelva el ToString() de las excepciones internas de InterpreterExceptions
            if (!ast.Statements.Any()) return true;
            try
            {
                foreach (var statement in ast?.Statements)
                {
                    RunStatement(statement);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                return false;
            }

        }

        private RunValue RunExpression(StatementExpression expression)
        {
            if (expression is NumericExpression n)
            {
                return new NumberRunValue(n.Literal);
            }
            else if (expression is StringExpression s)
            {
                return new StringRunValue(s.Literal);
            }
            else if (expression is BooleanExpression b)
            {
                return new BooleanRunValue(b.Boolean);
            }
            else if (expression is IdentifierExpression id)
            {
                if (!Globals.TryGetValue(id.Identifier, out var value))
                {
                    var v = Environment.Get(id.Identifier);
                    if (v != null) return v;
                    else throw new InterpreterExceptions.UndefinedVariableException(id.Identifier);
                }

                return value;
            }
            else if (expression is ListIndexExpression index)
            {
                var instance = RunExpression(index.Expression);
                var indexFromExpression = RunExpression(index.Index ?? throw new InterpreterExceptions.ExpectedIndexException("expected index")).to_number();
                var actualIndex = Convert.ToUInt32(indexFromExpression);

                if (instance is ListRunValue ls)
                {
                    var v = ls.Values.ElementAt((int)actualIndex);
                    if (v != null) return v;
                    throw new InterpreterExceptions.UndefinedIndexException(actualIndex.ToString());
                }

            }
            else if (expression is GetExpression get)
            {
                var instance = RunExpression(get?.Expression);
                if (instance is null) throw new InterpreterExceptions.ExpressionEvaluationException(get.Expression.ToString());

                var lit = get?.Field.ToString();
                return GetProperty(instance, get?.Field, get?.Expression);
            }
            else if (expression is AssignmentExpression assign)
            {
                var value = RunExpression(assign?.Right);

                if (assign.Left is ListIndexExpression lie)
                {
                    var instance = RunExpression(lie.Expression);

                    AssignValueToList(this, instance, lie?.Index, value);
                }
                else if (assign.Left is GetExpression ge)
                {
                    var instance = RunExpression(ge.Expression);
                    AssignValueToInstance(instance, ge.Field, value);
                }
                else
                {
                    if (assign.Left is IdentifierExpression ide)
                    {
                        Environment.Assign(ide.Identifier, value);
                    }
                    else { throw new NotImplementedException(); }

                }
                return value;
            }
            else if (expression is InfixExpression infix)
            {
                var left = RunExpression(infix?.Left);
                var right = RunExpression(infix?.Right);
                var op = infix?.Operand;

                if ((left, op, right) is (NumberRunValue l, Op.Add, NumberRunValue r))
                {
                    return new NumberRunValue(l.Value + r.Value);
                }
                else if ((left, op, right) is (NumberRunValue l1, Op.Multiply, NumberRunValue r1))
                {
                    return new NumberRunValue(l1.Value * r1.Value);
                }
                else if ((left, op, right) is (NumberRunValue l2, Op.Divide, NumberRunValue r2))
                {
                    return new NumberRunValue(l2.Value / r2.Value);
                }
                else if ((left, op, right) is (NumberRunValue l3, Op.Subtract, NumberRunValue r3))
                {
                    return new NumberRunValue(l3.Value - r3.Value);
                }
                else if ((left, op, right) is (NumberRunValue l4, Op.Add, StringRunValue r4))
                {
                    var _sb = new StringBuilder();
                    _sb.Append(l4);
                    _sb.Append(r4);

                    return new StringRunValue(_sb.ToString());
                }
                else if ((left, op, right) is (StringRunValue l5, Op.Add, NumberRunValue r5))
                {
                    var _sb = new StringBuilder();
                    _sb.Append(l5);
                    _sb.Append(r5);

                    return new StringRunValue(_sb.ToString());
                }
                else if ((left, op, right) is (StringRunValue l6, Op.Add, StringRunValue r6))
                {
                    var _sb = new StringBuilder();
                    _sb.Append(l6);
                    _sb.Append(r6);

                    return new StringRunValue(_sb.ToString());
                }
                else if ((left, op, right) is (StringRunValue l6b, Op.Add, BooleanRunValue r6b))
                {
                    var _sb = new StringBuilder();
                    _sb.Append(l6b);
                    _sb.Append(r6b.Value);

                    return new StringRunValue(_sb.ToString());
                }
                else if ((left, op, right) is (StringRunValue l7, Op.Equals, StringRunValue r7))
                {
                    return new BooleanRunValue(l7.Value == r7.Value);
                }
                else if ((left, op, right) is (NumberRunValue l8, Op.Equals, NumberRunValue r8))
                {
                    return new BooleanRunValue(l8.Value == r8.Value);
                }
                else if ((left, op, right) is (BooleanRunValue l9, Op.Equals, BooleanRunValue r9))
                {
                    return new BooleanRunValue(l9.Value == r9.Value);
                }
                else if ((left, op, right) is (StringRunValue l10, Op.NotEquals, StringRunValue r10))
                {
                    return new BooleanRunValue(l10.Value != r10.Value);
                }
                else if ((left, op, right) is (NumberRunValue l11, Op.NotEquals, NumberRunValue r11))
                {
                    return new BooleanRunValue(l11.Value != r11.Value);
                }
                else if ((left, op, right) is (BooleanRunValue l12, Op.NotEquals, BooleanRunValue r12))
                {
                    return new BooleanRunValue(l12.Value != r12.Value);
                }

                else if ((left, op, right) is (NumberRunValue l13, Op.LessThan, NumberRunValue r13))
                {
                    return new BooleanRunValue(l13.Value < r13.Value);
                }
                else if ((left, op, right) is (NumberRunValue l14, Op.GreaterThan, NumberRunValue r14))
                {
                    return new BooleanRunValue(l14.Value > r14.Value);
                }
                else if ((left, op, right) is (NumberRunValue l15, Op.LessThanOrEquals, NumberRunValue r15))
                {
                    return new BooleanRunValue(l15.Value <= r15.Value);
                }
                else if ((left, op, right) is (NumberRunValue l16, Op.GreaterThanOrEquals, NumberRunValue r16))
                {
                    return new BooleanRunValue(l16.Value >= r16.Value);
                }
                else if ((left, op, right) is (RunValue l17, Op.And, RunValue r17))
                {
                    return new BooleanRunValue(l17.to_bool() && r17.to_bool());
                }
                else if ((left, op, right) is (RunValue l18, Op.Or, RunValue r18))
                {
                    return new BooleanRunValue(l18.to_bool() || r18.to_bool());
                }
                else if ((left, op, right) is (NumberRunValue l19, Op.Pow, NumberRunValue r19))
                {
                    return new NumberRunValue(Math.Pow(l19.Value, r19.Value));
                }
                else if ((left, op, right) is (RunValue l20, Op.In, ListRunValue r20))
                {
                    return new BooleanRunValue(r20.Values.Any(x => x == left));
                }
                else if ((left, op, right) is (StringRunValue l21, Op.In, StringRunValue r21))
                {
                    return new BooleanRunValue(r21.Value.Contains(l21.Value));
                }
                else if ((left, op, right) is (RunValue l22, Op.NotIn, ListRunValue r22))
                {
                    return new BooleanRunValue(!r22.Values.Any(x => x == left));
                }
                else if ((left, op, right) is (StringRunValue l23, Op.In, StringRunValue r23))
                {
                    return new BooleanRunValue(!r23.Value.Contains(l23.Value));
                }


                throw new NotImplementedException();
            }
            else if (expression is ListExpression listEx)
            {
                var values = new List<RunValue>();
                foreach (var item in listEx.Items)
                {
                    values.Add(RunExpression(item));
                }
                return new ListRunValue(values);
            }
            else if (expression is ClosureExpression closureEx)
            {
                return new FunctionRunValue("Closure", closureEx?.Parameters, closureEx?.Body, Environment, null);
            }
            else if (expression is StructExpression structEx)
            {
                var definition = RunExpression(structEx?.Expression);
                StructRunValue structRunValue = default;
                if (definition is StructRunValue srv)
                {
                    structRunValue = srv;
                }
                else throw new NotImplementedException();

                var env = new InterpreterEnvironment();
                foreach (var item in structEx?.Fields)
                {
                    if (!structRunValue.Fields.Any(x => x.Name.Contains(item.Key))) throw new InterpreterExceptions.UndefinedFieldException(string.Format("{0} {1}", structRunValue?.Name, item.Key));
                    var value = RunExpression(item.Value);
                    RunValue runValue = default;
                    if (value is StructInstanceRunValue structInstanceRunValue)
                    {
                        runValue = new StructInstanceRunValue(structInstanceRunValue?.Environment, structInstanceRunValue?.Definition);
                    }
                    else
                    {
                        runValue = value;
                    }
                    env.Set(item.Key, runValue);
                }

                env = new InterpreterEnvironment();

                foreach (var item in srv.Methods)
                {
                    RunValue method = default;
                    if (item.Value is FunctionRunValue functionRunValue)
                    {
                        method = new FunctionRunValue(functionRunValue.Name, functionRunValue?.Parameters, functionRunValue?.Block, null, null);
                    }

                    else
                    {
                        throw new NotImplementedException();
                    }

                    env.Set(item.Key, method);
                }

                return new StructInstanceRunValue(env, definition);
            }
            else if (expression is CallExpression call)
            {
                var callable = RunExpression(call.Expression);
                var argumentValues = new List<RunValue>();
                foreach (var item in call?.Args)
                {
                    argumentValues.Add(RunExpression(item));
                }
                return Call(callable, argumentValues);
            }
            else if (expression is PrefixExpression prefix)
            {
                var right = RunExpression(prefix.Expression);

                if (prefix.Op == Op.Bang)
                {
                    return new BooleanRunValue(!right.to_bool());
                }
                else if (prefix.Op == Op.Subtract)
                {
                    return new NumberRunValue(-right.to_number());
                }
                else throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
        private void AssignValueToInstance(RunValue instance, string field, RunValue value)
        {
            if (instance is StructInstanceRunValue sirv)
            {
                sirv.Environment.Set(field, value);
                return;
            }
            else if (instance is StructRunValue srv)
            {
                if (value is not FunctionRunValue)
                {
                    throw new InterpreterExceptions.InvalidMethodAssignmentException(value.typestring());
                }
                srv.Methods.Add(field, value);
                return;
            }
            //else if()
            throw new InterpreterExceptions.InvalidMethodAssignmentException(value.typestring());
        }
        private void AssignValueToList(Interpreter interpreter, RunValue instance, StatementExpression? index, RunValue value)
        {
            if (instance is ListRunValue lrv)
            {
                if (index != null)
                {
                    var i = (int)interpreter.RunExpression(index).to_number(); //esto podría lanzar una excepción, pero vamos a ver
                    lrv.Values[i] = value;
                    return;
                }
                lrv.Values.Add(value);
                return;
            }

            throw new InterpreterExceptions.InvalidAppendTargetException(instance.typestring());
        }
        private void RunStatement(Statement statement)
        {

            if (statement != null)
            {
                if (statement is VarStatement varStatement)
                {
                    if (varStatement?.Expression is null) Environment.Set(varStatement?.Name, new NullRunValue());
                    else
                    {
                        var value = RunExpression(varStatement?.Expression);
                        Environment.Set(varStatement?.Name, value);
                    }
                    return;
                }
                else if (statement is FunctionDeclarationStatement functionDeclarationStatement)
                {
                    Globals.Add(functionDeclarationStatement?.Name, new FunctionRunValue(functionDeclarationStatement?.Name, functionDeclarationStatement?.Parameters, functionDeclarationStatement?.Body, null, null));
                    return;
                }
                else if (statement is StructDeclarationStatement structDeclarationStatement)
                {
                    Globals.Add(structDeclarationStatement?.Name, new StructRunValue(structDeclarationStatement?.Name, structDeclarationStatement?.Fields, new Dictionary<string, RunValue>()));
                    return;
                }
                else if (statement is ForEachStatement forEachStatement)
                {
                    var iterable = RunExpression(forEachStatement?.Iterable);
                    var items = new List<RunValue>();
                    if (iterable is ListRunValue listRunValue) items = listRunValue.Values;
                    else throw new InterpreterExceptions.InvalidIterableException(iterable.typestring());

                    if (!items.Any()) return;

                    var indexIsSet = forEachStatement?.Index != null;
                    for (int i = 0; i < items.Count; i++)
                    {
                        Environment.Assign(forEachStatement?.Value, items[i]);
                        if (indexIsSet) Environment.Assign(forEachStatement?.Index, new NumberRunValue((double)i));

                        foreach (var stmt in forEachStatement?.Block)
                        {
                            RunStatement(stmt);
                        }
                    }

                    Environment.Drop(forEachStatement?.Value);
                    if (indexIsSet) Environment.Drop(forEachStatement?.Index);

                    return;

                }
                else if (statement is WhileStatement whileStatement)
                {
                    while (RunExpression(whileStatement?.Condition).to_bool())
                    {
                        foreach (var stmt in whileStatement?.Block)
                        {
                            RunStatement(stmt);
                        }
                    }
                    return;
                }
                else if (statement is DoWhileStatement doWhileStatement)
                {
                    do
                    {
                        foreach (var stmt in doWhileStatement?.Block)
                        {
                            RunStatement(stmt);
                        }
                    } while (RunExpression(doWhileStatement?.Condition).to_bool());
                    return;
                }
                else if (statement is ForStatement forStatement)
                {
                    RunStatement(forStatement?.Init);
                    while (RunExpression(forStatement?.Condition).to_bool())
                    {
                        foreach (var stmt in forStatement?.Block)
                        {
                            RunStatement(stmt);
                        }
                        RunStatement(forStatement?.Post);
                    }
                    if (forStatement?.Init is VarStatement forInitVar)
                    {
                        Environment.Drop(forInitVar.Name);
                    }
                    return;
                }
                else if (statement is IfStatement ifStatement)
                {
                    var condition = RunExpression(ifStatement?.Condition);
                    if (condition.to_bool())
                    {
                        foreach (var stmt in ifStatement?.Then)
                        {
                            RunStatement(stmt);
                        }
                    }
                    else
                    {
                        foreach (var stmt in ifStatement?.Otherwise)
                        {
                            RunStatement(stmt);
                        }
                    }
                    return;
                }
                else if (statement is ExpressionStatement expressionStatement)
                {
                    RunExpression(expressionStatement.Expression);

                    return;
                }
                else if (statement is ReturnStatement returnStatement)
                {
                    var t = RunExpression(returnStatement?.Expression);
                    if (t is null) throw new InterpreterExceptions.ReturnValueException();
                    return;
                }
                throw new NotImplementedException();
            }


        }
        private void DefineGlobalFunction(string name, NativeFunctionCallback callback)
        {
            Globals.Add(name, new NativeFunctionRunValue(name, callback));
        }

        private RunValue GetProperty(RunValue value, string field, StatementExpression target)
        {
            if (value is StructInstanceRunValue structInstance)
            {
                var fieldValue = Environment.Get(field);
                if (fieldValue != null)
                {
                    if (fieldValue is FunctionRunValue functionRunValue)
                    {
                        return new FunctionRunValue(functionRunValue?.Name, functionRunValue?.Parameters, functionRunValue?.Block, functionRunValue?.Environment, functionRunValue?.Context);
                    }
                    return fieldValue;
                }
                else
                {
                    var name = string.Empty;
                    if (structInstance?.Definition is StructRunValue st)
                    {
                        name = st?.Name;
                    }
                    else
                    {
                        throw new Exception();
                    }

                    throw new InterpreterExceptions.UndefinedFieldException(string.Format("Name: {0} Field: {1}", name, field));
                }
            }
            else if (value is StructRunValue str)
            {
                if (!str.Methods.TryGetValue(field, out var methodValue))
                {
                    throw new InterpreterExceptions.UndefinedMethodException(string.Format("Name: {0} Field: {1}", str.Name, field));
                }
                return methodValue;

            }
            else if (value is StringRunValue strv)
            {
                var _so = new StringObject();
                var v = new NativeMethodRunValue(field, _so.Get(field.Trim()), target);
                return v;
            }
            else if (value is NumberRunValue nrv)
            {
                var _no = new NumberObject();
                var v = new NativeMethodRunValue(field, _no.Get(field), target);
                return v;
            }
            else if (value is ListRunValue lrv)
            {
                var _lo = new ListObject();
                var v = new NativeMethodRunValue(field, _lo.Get(field), target);
                return v;
            }
            //agregar constantes más adelante...

            throw new NotImplementedException();
        }
    }
}
