using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using FSQL.Grammar;
using FSQL.Interfaces;
using FSQL.ProgramParts;
using FSQL.ProgramParts.Core;
using FSQL.ProgramParts.DataTypes;
using FSQL.ProgramParts.Expressions;
using FSQL.ProgramParts.FSQLStructs;
using FSQL.ProgramParts.FSQLStructs.DataSetFormatters;
using FSQL.ProgramParts.FSQLStructs.Destinations;
using FSQL.ProgramParts.FSQLStructs.SelectObjects;
using FSQL.ProgramParts.FSQLStructs.Sources;
using FSQL.ProgramParts.FSQLStructs.WhereClauses;
using FSQL.ProgramParts.Functions;
using FSQL.ProgramParts.Statements;
using FSQL.ProgramParts.Variables;

namespace FSQL {

    public class ExpressionWrapper : StatementBase {
        private readonly IExpression _expr;

        public ExpressionWrapper(IExpression expr) {
            _expr = expr;
        }

        public override void Dispose() {}
        protected override object OnExecute(IExecutionContext ctx, params object[] parms) {
            return _expr.GetGenericValue(ctx);
        }
    }

    public class FSQLEngine : FSQLParserBaseVisitor<IProgramPart>, IScriptEngine
    {
        #region Interface
        public static IScriptEngine Create() {
            return new FSQLEngine();
        }

        public IScript Build(string script, Action<IExecutionContext> contextInitializer = null) {
            var inputStream = new AntlrInputStream(script);
            var lexer = new FSQLLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new FSQLParser(commonTokenStream);

            var examContext = parser.script();

            var results = Visit(examContext);
            return new FSQLScript(results as IExecutable, contextInitializer);
        }

        public dynamic Execute(IScript script, params object[] parms) {
            var result = script.InvokeGeneric(parms);
            return result;
        }

        public dynamic Execute(string script, params object[] parms) =>
            Execute(Build(script), parms);

        public dynamic Execute(string script, Action<IExecutionContext> initializer, params object[] parms) =>
            Execute(Build(script, initializer), parms);

        public IStatement Parse(string source) {
            var inputStream = new AntlrInputStream(source);
            var lexer = new FSQLLexer(inputStream);
            var commonTokenStream = new CommonTokenStream(lexer);
            var parser = new FSQLParser(commonTokenStream);

            IStatement results = new Unimplemented(source);

            var stmtContext = parser.statement();
            

            //IProgramPart parseResults;
            try {
                results = Visit(stmtContext) as IStatement;
                if (results == null) throw new SyntaxErrorException($"Could not parse '{source}'.");
            } catch (Exception) {
                if (!source.StartsWith("@_ =")) {
                    source = "@_ = " + source;
                    if (!source.EndsWith(";")) source = source + ";";
                    results = Parse(source);
                } else {
                    throw;
                }
            }
                       
            return results;
        }


        public IExecutionContext Debug(string script, params object[] parms) => Debug(script, null, parms);
       

        public IExecutionContext Debug(string script, Action<IExecutionContext> contextInitializer, params object[] parms) 
        {
            var program = Build(script, contextInitializer);
            var ctx = program.Debug(parms);
            return ctx;
        }

        public dynamic Run(string filename, params object[] parms) => Execute(File.ReadAllText(filename), parms);
        public dynamic Run(string filename, Action<IExecutionContext> contextInitializer, params object[] parms) 
            => Execute(File.ReadAllText(filename), contextInitializer, parms);

        #endregion

        #region FSQL Functions

        public override IProgramPart VisitSimulateStatement(FSQLParser.SimulateStatementContext context) {
            var body = Visit(context.body) as CodeBlock;
            return new Simulate(body);
        }

        public override IProgramPart VisitMapFolderTo(FSQLParser.MapFolderToContext context) {
            var aliasname = context.alias.Text;
            //var folder = UnescapeString(context.folder.Text);
            var folder = Visit(context.folder) as IExpression;            
            //var filter = UnescapeString(context.filter?.Text ?? "*.*");
            var filter = context.filter == null ? (StringConstant) "*.*" : Visit(context.filter) as IExpression;
            var mapTo = new MapTo(aliasname, folder, filter);
            return mapTo;
            //return base.VisitMapFolderTo(context);
        }

        public override IProgramPart VisitUnmapStatement(FSQLParser.UnmapStatementContext context) {
            var aliasName = context.IDENT().GetText();
            return new UnMap(aliasName);
        }

        public override IProgramPart VisitInsertInto(FSQLParser.InsertIntoContext context) {
            var target = Visit(context.target) as AliasRef;
            var source = Visit(context.source) as SelectFrom;
            return new SelectInto(target, source, false);            
        }

        public override IProgramPart VisitSelectInto(FSQLParser.SelectIntoContext context) {
            var target = Visit(context.target) as AliasRef;
            var source = Visit(context.source) as SelectFrom;
            return new SelectInto(target, source, true);            
        }

        public override IProgramPart VisitDeleteFrom(FSQLParser.DeleteFromContext context) {
            var target = Visit(context.target) as AliasRef;
            var where = context.where != null ? Visit(context.where) as WhereClauseBase : new NullWhereClause();
            return new DeleteFrom(target, where);            
        }

        public override IProgramPart VisitFSQLSelect(FSQLParser.FSQLSelectContext context) {
            var select = Visit(context.basicSelect()) as SelectFrom;

            var selectedFormat = (context.formatting == null) ? (StringConstant) "NONE" : Visit(context.formatting);

            var formatter = DataSetFormatterFactory.Build(selectedFormat.ToString(), select);

            var selectedOutput = (context.output == null) ? (StringConstant) "NONE" : Visit(context.output) as IExpression;
            
            var destination = DestinationFactory.Build(selectedOutput, formatter);

            return destination;
        }

        public override IProgramPart VisitBasicSelect(FSQLParser.BasicSelectContext context) {
            var attributes = Visit(context.attributes) as ProgramPartList<ScopedAttribute>;
            var source = Visit(context.sources) as Source;
            var whereClause = context.where != null ? Visit(context.where) as WhereClauseBase : new NullWhereClause();

            var select = new SelectFrom(attributes, source, whereClause);
            return select;
        }

        #region Attributes

        public override IProgramPart VisitGetAttributeList(FSQLParser.GetAttributeListContext context) {            
            var list = (context.optional != null) 
                ? Visit(context.optional) as ProgramPartList<ScopedAttribute>
                : new ProgramPartList<ScopedAttribute>();
                        
            var ta = Visit(context.current) as ScopedAttribute;
            if (ta.PropertyName == "*") {
                list.Insert(0, new ScopedAttribute(ta.ObjectName, "Path", "Path"));
                list.Insert(0, new ScopedAttribute(ta.ObjectName, "Updated", "Updated"));
                list.Insert(0, new ScopedAttribute(ta.ObjectName, "Created", "Created"));
                list.Insert(0, new ScopedAttribute(ta.ObjectName, "Size", "Size"));
                list.Insert(0, new ScopedAttribute(ta.ObjectName, "Extension", "Extension"));
                list.Insert(0, new ScopedAttribute(ta.ObjectName, "NameOnly", "NameOnly"));                
            } else list.Insert(0, ta);
            return list;
        }

        public override IProgramPart VisitFSQLScopedAttribute(FSQLParser.FSQLScopedAttributeContext context) {            
            var reference = Visit(context.@ref) as ScopedAttribute;
            var colName = context.colName?.Text;
            return new ScopedAttribute(reference.ObjectName, reference.PropertyName, colName);
        }

        public override IProgramPart VisitFSQLScopedAttributeRef(FSQLParser.FSQLScopedAttributeRefContext context) {
            var obj = context.obj?.Text;
            var prop = context.prop?.GetText();
            if (prop.ToLowerInvariant() == "checksum") Settings.EnableChecksums = true;
            return new ScopedAttribute(obj, prop, prop);
        }


        public override IProgramPart VisitIdentifierWithScopedAttribute(FSQLParser.IdentifierWithScopedAttributeContext context) {
            var ident = Visit(context.attr);
            return ident;
        }

        #region Column Names (Obsolete)
        //public override IProgramPart VisitFullNameAttribute(FSQLParser.FullNameAttributeContext context) => (StringConstant) "FullName";
        //public override IProgramPart VisitFileNameAttribute(FSQLParser.FileNameAttributeContext context) => (StringConstant)"FileName";
        //public override IProgramPart VisitPathAttribute(FSQLParser.PathAttributeContext context) => (StringConstant) "Path";
        //public override IProgramPart VisitNameOnlyAttribute(FSQLParser.NameOnlyAttributeContext context) => (StringConstant)"NameOnly";
        //public override IProgramPart VisitExtensionAttribute(FSQLParser.ExtensionAttributeContext context) => (StringConstant)"Extension";
        //public override IProgramPart VisitSizeAttribute(FSQLParser.SizeAttributeContext context) => (StringConstant) "Size";
        //public override IProgramPart VisitCreatedAttribute(FSQLParser.CreatedAttributeContext context) => (StringConstant) "Created";
        //public override IProgramPart VisitUpdatedAttribute(FSQLParser.UpdatedAttributeContext context) => (StringConstant) "Updated";
        //public override IProgramPart VisitChecksumAttribute(FSQLParser.ChecksumAttributeContext context) {
        //    Settings.EnableChecksums = true;
        //    return (StringConstant) "CheckSum";
        //}

        //public override IProgramPart VisitAllAttributes(FSQLParser.AllAttributesContext context) => (StringConstant) "*";
        #endregion

        #endregion

        #region From Clause
        public override IProgramPart VisitFromTargetList(FSQLParser.FromTargetListContext context) {
            var x =  Visit(context.currentTarget);
            return x;
        }

        public override IProgramPart VisitImplementJoinClause(FSQLParser.ImplementJoinClauseContext context) => Visit(context.join);
        
        public override IProgramPart VisitComplexJoinClause(FSQLParser.ComplexJoinClauseContext context) {
            var left = Visit(context.left);
            var right = Visit(context.right);
            var joinCond = Visit(context.expr);
            var newName = context.newAlias?.Text;
            return new JoinSourceAggregator(left as AliasRef, right as AliasRef, joinCond as IExpression, newName);
        }

        public override IProgramPart VisitSimpleJoinClause(FSQLParser.SimpleJoinClauseContext context) {
            var alias = Visit(context.left) as AliasRef;
            return new SingleSourceAggregator(alias);
        }

        public override IProgramPart VisitFileSpecString(FSQLParser.FileSpecStringContext context) {
            string alias = null;
            string folder = context.STRING().GetText().Unescape();
            return new AliasRef(alias, folder);
        }

        public override IProgramPart VisitFileSpecAlias(FSQLParser.FileSpecAliasContext context) {
            string alias = context.IDENT().GetText();
            string folder = null;
            return new AliasRef(alias, folder);
        }

        public override IProgramPart VisitFileSpecWithAlias(FSQLParser.FileSpecWithAliasContext context) {
            string alias = context.IDENT().GetText();
            string folder = context.STRING().GetText().Unescape();
            return new AliasRef(alias, folder);
        }

        #endregion

        #region Where Clause

        public override IProgramPart VisitWhereInSubquery(FSQLParser.WhereInSubqueryContext context) {
            var isNegated = context.isNegated != null;
            ScopedAttribute testValue = Visit(context.value) as ScopedAttribute;
            var subQuery = (SelectFrom) (Visit(context.subQuery) as IDataSetFormatter).DataSetRetriever; // Ignore any formatter... they shouldn't be there!
            return new WhereInSubqueryClause(testValue, subQuery, isNegated);
        }

        public override IProgramPart VisitWhereBoolExpression(FSQLParser.WhereBoolExpressionContext context) {
            var expr = Visit(context.expr) as IExpression;
            return new WhereClause(expr);
        }

        #endregion

        #region Format Clause

        public override IProgramPart VisitFormatClause(FSQLParser.FormatClauseContext context) => Visit(context.formatSpec);        
        public override IProgramPart VisitFormatAsCSV(FSQLParser.FormatAsCSVContext context) => (StringConstant) "CSV";
        public override IProgramPart VisitFormatAsJSON(FSQLParser.FormatAsJSONContext context) => (StringConstant)"JSON";
        public override IProgramPart VisitFormatAsText(FSQLParser.FormatAsTextContext context) => (StringConstant)"TEXT";
        public override IProgramPart VisitFormatAsScalar(FSQLParser.FormatAsScalarContext context) => (StringConstant)"SCALAR";
        #endregion

        #region Output clause

        public override IProgramPart VisitOutputClause(FSQLParser.OutputClauseContext context) => Visit(context.outputSpec);

        public override IProgramPart VisitOutputToScreen(FSQLParser.OutputToScreenContext context) => (StringConstant) "SCREEN";
        public override IProgramPart VisitOutputToClipboard(FSQLParser.OutputToClipboardContext context) => (StringConstant) "CLIPBOARD";
        public override IProgramPart VisitOutputToFile(FSQLParser.OutputToFileContext context) {
            //var filename = context.filename.Text.Unescape();
            var filename = Visit(context.filename) as IExpression;
            return (IExpression) filename;
        }

        #endregion

        #endregion

        #region Functions

        public override IProgramPart VisitFunctionDef(FSQLParser.FunctionDefContext context) {
            var name = context.IDENT().GetText();
            //Console.WriteLine("DECL FUNCTION " + name);
            var parms = Visit(context.parms) as IList<string>;
            var body = Visit(context.body);
            return new Function(name, parms, body as IStatement);
        }


        public override IProgramPart VisitWrappedParameters(FSQLParser.WrappedParametersContext context) => 
              context.block != null 
            ? Visit(context.block)
            : new ProgramPartList<IExpression>();

        public override IProgramPart VisitWrappedArguments(FSQLParser.WrappedArgumentsContext context) =>
              context.block != null
            ? Visit(context.block)
            : new ProgramTerminalList<string>();

        public override IProgramPart VisitFunctionParameters(FSQLParser.FunctionParametersContext context) {
            var list = (context.next != null)
                ? Visit(context.next) as ProgramTerminalList<string>
                : new ProgramTerminalList<string>();

            var parm = Visit(context.current) as VariableGet;
            list.Insert(0, parm.Name);
            return list;
        }

        public override IProgramPart VisitExistsFunction(FSQLParser.ExistsFunctionContext context) {
            var varName = context.variable.GetText().Trim("@".ToCharArray());
            return new Exists(varName);
        }

        public override IProgramPart VisitFunctionCall(FSQLParser.FunctionCallContext context) {
            var isInternal = context.fnInt != null && context.fnUsr == null;                   
            var name = isInternal ? context.fnInt.GetText() : context.IDENT().GetText();
            var args = Visit(context.args) as IEnumerable<IExpression>;
            if (isInternal) {
                IProgramPart results;
                var token = context.fnInt.GetChild(0).Payload as IToken;
                var type = token?.Type ?? -1;
                switch (type) {
                    case FSQLLexer.WRITELINE:
                        results = new WriteLine(args);
                        break;
                    case FSQLLexer.WRITE:
                        results = new Write(args);
                        break;
                    case FSQLLexer.COREDUMP:
                        results = new SystemFn("CoreDump", (ctx, prms) => ctx.Dump(Priority.Verbose));
                        break;
                    case FSQLLexer.READLINE:
                        results = new SystemFn("ReadLine", (ctx, prms) => Console.ReadLine());
                        break;
                    case FSQLLexer.READ:
                        results = new SystemFn("Read", (ctx, prms) => Console.Read());
                        break;
                    //case FSQLLexer.EXISTS:
                    //    results = new Exists(args);
                    //    break;
                    default:
                        results = new Unimplemented(name);
                        break;
                }
                return results;
            } 
            return new FunctionCall(name, args);
            
        }

        public override IProgramPart VisitFunctionArguments(FSQLParser.FunctionArgumentsContext context) {
            var list = (context.next != null)
                ? Visit(context.next) as ProgramPartList<IExpression>
                : new ProgramPartList<IExpression>();

            var parm = Visit(context.current) as IExpression;
            list.Insert(0, parm);
            return list;
        }

        public override IProgramPart VisitCodeblock(FSQLParser.CodeblockContext context)
        {

            //Console.WriteLine("CB: " + context.body.GetText());
            var results = new List<IStatement>();
            foreach (var child in context.children.Skip(1).Take(context.ChildCount - 2)) {
                if (child.GetText() == "{") continue;
                if (child.GetText() == "}") continue;
                var statementMaybe = Visit(child) as IProgramPart;

                if (statementMaybe.Type == PartType.Statement || 
                    statementMaybe.Type == PartType.Variable ||
                    statementMaybe.Type == PartType.Function) {

                    //Console.WriteLine("CB-C>: " + child.GetText());
                    results.Add(statementMaybe as IStatement);

                }
            }
            return new CodeBlock("CB", results);
        }

        public override IProgramPart VisitExportStatement(FSQLParser.ExportStatementContext context) {
            var pBlock = Visit(context.paramblock()) as ProgramTerminalList<string>;
            var exporter = new ExportStatement(pBlock);
            return exporter;
        }

        public override IProgramPart VisitReturnStatement(FSQLParser.ReturnStatementContext context) {
            var valueGenerator = Visit(context.expression());
            var result = new ReturnStatement(valueGenerator);
            return result;
        }

        #region Supporting Features

        public override IProgramPart VisitParamblock(FSQLParser.ParamblockContext context) {
            var pList = new List<string>();
            foreach (var prm in context.children.Skip(1).Take(context.ChildCount - 2)) {
                pList.Add(prm.GetText());
            }
            return new ParamBlock(pList.ToArray());
        }

        #endregion


        #endregion

        #region Conditionals

        #region While Statement

        public override IProgramPart VisitWhileStatement(FSQLParser.WhileStatementContext context) {
            var cond = Visit(context.cond) as IExpression;
            var body = Visit(context.body) as CodeBlock;
            return new WhileStatement(cond, body);
        }
        #endregion

        #region If Statement

        public override IProgramPart VisitIfStatement(FSQLParser.IfStatementContext context) {
            var cond = Visit(context.cond) as IExpression;
            var trueBlock = Visit(context.trueblock) as CodeBlock;
            var falseBlock = context.falseblock != null ? Visit(context.falseblock) as CodeBlock : null;
            return new IfStatement(cond, trueBlock, falseBlock);
        }

        #endregion

        #endregion

        #region Expressions

        #region Mathematics

        public override IProgramPart VisitUnarySign(FSQLParser.UnarySignContext context) {
            var right = Visit(context.right) as IExpression;
            var op = context.op;
            return op.Type != FSQLLexer.MINUS
                ? right
                : OneTermExpression.Create(right, r => r*-1, "-");
        }

        public override IProgramPart VisitNumParens(FSQLParser.NumParensContext context) =>
            Visit(context.expr);

        public override IProgramPart VisitRaiseToPower(FSQLParser.RaiseToPowerContext context) =>
            Expr(context, "^", (l, r) => Math.Pow(Convert.ToDouble(l), Convert.ToDouble(r)));

        public override IProgramPart VisitMultiply(FSQLParser.MultiplyContext context) =>
            Expr(context, "*", (l, r) => l*r);

        public override IProgramPart VisitDivide(FSQLParser.DivideContext context) =>
            Expr(context, "/", (l, r) => l / r);

        public override IProgramPart VisitAdd(FSQLParser.AddContext context) =>
            Expr(context, "+", (l, r) => l + r);

        public override IProgramPart VisitSubtract(FSQLParser.SubtractContext context) =>
            Expr(context, "-", (l, r) => l - r);

        protected IProgramPart Expr(dynamic ctx, string oper, Func<dynamic, dynamic, dynamic> xformer)
        {
            var left = Visit(ctx.left) as IExpression;
            if (ctx.right == null) return left;
            var right = Visit(ctx.right) as IExpression;
            return TwoTermExpression.Create(left, right, xformer, oper);
        }

        protected IProgramPart Expr<T>(dynamic ctx, Func<T, T, T> xformer) {
            var left = Visit(ctx.left) as IExpression<T>;
            if (ctx.right == null) return left;
            var right = Visit(ctx.right) as IExpression<T>;
            return TwoTermExpression<T>.Create<T>(left, right, xformer);
        }


        #endregion

        #region Booleans

        public override IProgramPart VisitInvertBoolean(FSQLParser.InvertBooleanContext context) {
            var right = Visit(context.boolExpr) as IExpression;
            return OneTermExpression.Create(right, r => !r, "NOT ");
        }    

        public override IProgramPart VisitBoolParens(FSQLParser.BoolParensContext context) => Visit(context.expr);

        public override IProgramPart VisitBooleanAnd(FSQLParser.BooleanAndContext context) =>
            Expr(context, "AND", (l, r) => l && r);

        public override IProgramPart VisitBooleanOr(FSQLParser.BooleanOrContext context) =>
            Expr(context, "OR", (l, r) => l || r);

        #region Numeric Comparators

        public override IProgramPart VisitNumberIsEqualTo(FSQLParser.NumberIsEqualToContext context) =>
            Expr(context, "==", (l, r) => l == r);

        public override IProgramPart VisitNumberIsGreater(FSQLParser.NumberIsGreaterContext context) =>
            Expr(context, ">", (l, r) => l > r);

        public override IProgramPart VisitNumberIsGreaterOrEqual(FSQLParser.NumberIsGreaterOrEqualContext context) =>
            Expr(context, ">=", (l, r) => l >= r);

        public override IProgramPart VisitNumberIsLessOrEqual(FSQLParser.NumberIsLessOrEqualContext context) =>
            Expr(context, "<", (l, r) => l < r);

        public override IProgramPart VisitNumberIsLesser(FSQLParser.NumberIsLesserContext context) =>
            Expr(context, "<=", (l, r) => l <= r);


        #endregion

        #endregion

        #region Strings

        public override IProgramPart VisitStringConcat(FSQLParser.StringConcatContext context) =>
            Expr(context, "+", (l, r) => l + r);

        #endregion

        #endregion

        #region Variables

        public override IProgramPart VisitAssignVariable(FSQLParser.AssignVariableContext context) {
            //var name = (((FSQLParser.IdentifierWithAtContext)(context.id)).id).Text;
            var variable = Visit(context.id);
            string name = "_";
            if (variable is VariableGet) {
                name = (variable as VariableGet).Name;
            } else if (variable is StringConstant) {
                name = variable.ToString();
            }
            var expression = Visit(context.expr) as IExpression;
            return new VariableSet(name, expression);
        }

        public override IProgramPart VisitIdentifierWithAt(FSQLParser.IdentifierWithAtContext context) {
            var name = context.IDENT().GetText();
            return new VariableGet(name);
        }

        public override IProgramPart VisitInternalIdentifier(FSQLParser.InternalIdentifierContext context) {
            return (StringConstant) "_";
        }

        #endregion

        #region Constant Values

        public override IProgramPart VisitTrueConstant(FSQLParser.TrueConstantContext context) => BoolConstant.True;
        public override IProgramPart VisitFalseConstant(FSQLParser.FalseConstantContext context) => BoolConstant.False;
        public override IProgramPart VisitInt(FSQLParser.IntContext context) => (IntConstant) Convert.ToInt32(context.UINT().GetText());
        public override IProgramPart VisitNumber(FSQLParser.NumberContext context) => (DoubleConstant) Convert.ToDouble(context.NUMBER().GetText());
        public override IProgramPart VisitStringLiteral(FSQLParser.StringLiteralContext context) => (StringConstant) context.STRING().GetText().Trim("\"".ToCharArray());

        #endregion

        #region Misc
        public override IProgramPart VisitSemicolonedStatement(FSQLParser.SemicolonedStatementContext context) => Visit(context.stmt);



        #endregion
        
        #region Debugging

        public override IProgramPart VisitToggleTracing(FSQLParser.ToggleTracingContext context) {            
            return context.on != null ? new TraceOn() as IProgramPart : new TraceOff();
        }

        public override IProgramPart VisitChildren(IRuleNode node)
        {
            return base.VisitChildren(node);
        }

        public override IProgramPart Visit(IParseTree tree) {
            var x = base.Visit(tree);
            //Console.WriteLine(tree.GetText());
#pragma warning disable 162
            if (false) {
                const int col = -30;
                try {
                    Console.WriteLine($"{tree.GetType().Name,col}|{x.GetType().Name,col}|{x}");
                } catch {
                    Console.WriteLine($"{tree.GetType().Name,col}|{x?.GetType().Name,col}|{(x?.ToString() ?? "NULL")}");
                }
            }
#pragma warning restore 162
            return x;
        }



        #endregion

    }
}