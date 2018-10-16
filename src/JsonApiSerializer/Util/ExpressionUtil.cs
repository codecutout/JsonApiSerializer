using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace JsonApiSerializer.Util
{
    internal class ExpressionUtil
    {
        /// <summary>
        /// Provides utility for evaluating expressions.
        /// </summary>
        private class ExpressionEvaluator : ExpressionVisitor
        {
            /// <summary>
            /// Determines whether an expression can be evaluated locally
            /// </summary>
            /// <param name="exp">The expression to evaluate locally.</param>
            /// <returns>
            ///   <c>true</c> if this instance can be evaluated locallay; otherwise, <c>false</c>.
            /// </returns>
            public static bool TryEvaluate<T>(Expression exp, out T result)
            {
                bool shouldBox = false;

                //check the the expression will evaluate to hte type we want to get out
                if (exp.Type != typeof(T) && typeof(T).GetTypeInfo().IsAssignableFrom(exp.Type.GetTypeInfo()))
                {
                    if (typeof(T) == typeof(object))
                    {
                        //we can box our non-object to coerce the result as an object
                        shouldBox = true;
                    }
                    else
                    {
                        //our types are just not compatible so stop here
                        result = default(T);
                        return false;
                    }
                }

                //if its a constant we can avoid compiling a lambda and get the value directly
                if (exp is ConstantExpression constExp)
                {
                    result = (T)constExp.Value;
                    return true;
                }

                //check to see if we can evaluate
                if (!ExpressionEvaluator.CanEvaluate(exp))
                {
                    result = default(T);
                    return false;
                }

                try
                {
                    if (shouldBox)
                        exp = Expression.Convert(exp, typeof(object));

                    var lambdaExp = Expression.Lambda<Func<T>>(exp);
                    var lambda = lambdaExp.Compile();
                    result = lambda();
                    return true;
                }
                catch (Exception)
                {
                    //we shouldnt get here, but if we do its because something is wrong
                    //with the expression, which indictes we cannot evaluate it locally
                    result = default(T);
                    return false;
                }
            }


            /// <summary>
            /// Determines whether an expression can be evaluated locally
            /// </summary>
            /// <param name="exp">The expression to evaluate locally.</param>
            /// <returns>
            ///   <c>true</c> if this instance can be evaluated locallay; otherwise, <c>false</c>.
            /// </returns>
            public static bool CanEvaluate(Expression exp)
            {
                var evaluator = new ExpressionEvaluator();
                evaluator.Visit(exp);
                return evaluator.CanEvaluateLocally;
            }

            public bool CanEvaluateLocally { get; private set; } = true;

            private ExpressionEvaluator() { }

            public override Expression Visit(Expression node)
            {
                //if its using a parameter it means we can not evaulate the locally
                CanEvaluateLocally &= node.NodeType != ExpressionType.Parameter;
                if (!CanEvaluateLocally)
                    return node;
                return base.Visit(node);
            }
        }

        /// <summary>
        /// Provides utility for reading a path of property access
        /// </summary>
        private class PropertyPathReader : ExpressionVisitor
        {
            public static bool TryReadPropertyPath(Expression exp, out IEnumerable<Expression> path)
            {
                var ppr = new PropertyPathReader();
                try
                {
                    ppr.Visit(exp);
                    path = ppr.Path;
                    return ppr.Success;
                }
                catch
                {
                    path = default(IEnumerable<Expression>);
                    return false;
                }
            }

            public List<Expression> Path { get; private set; } = new List<Expression>();
            public bool Success { get; private set; } = true;

            private PropertyPathReader() { }

            public override Expression Visit(Expression node)
            {
                switch (node)
                {
                    case MemberExpression me:
                        Visit(me.Expression);
                        Path.Add(node);
                        return node;
                    case BinaryExpression be when node.NodeType == ExpressionType.ArrayIndex:
                        Visit(be.Left);
                        Path.Add(node);
                        return node;
                    case MethodCallExpression mce:
                        Visit(mce.Object);
                        Path.Add(node);
                        return node;
                    case IndexExpression ie:
                        Visit(ie.Object);
                        Path.Add(node);
                        return node;
                    case ParameterExpression pe:
                        Path.Add(node);
                        return node;
                    default:
                        Success = false;
                        return node;
                }
            }
        }

        /// <summary>
        /// Determines if an expression represents an index access
        /// </summary>
        /// <param name="exp">The expression to check</param>
        /// <param name="indexExpression">The indexer used in the in the index access</param>
        /// <returns><c>true</c> if expression was an index access; otherwise, <c>false</c>.</returns>
        public static bool IsIndexAccess(Expression exp, out Expression indexExpression)
        {
            //Expression can have several ways to indicate an index access, so have to check them all
            switch (exp)
            {
                case BinaryExpression be when be.NodeType == ExpressionType.ArrayIndex:
                    indexExpression = be.Right;
                    return true;
                case MethodCallExpression mce when IsDefaultMemberGet(mce) && mce.Arguments.Count == 1:
                    indexExpression = mce.Arguments[0];
                    return true;
                case IndexExpression ie when ie.Arguments.Count == 1:
                    indexExpression = ie.Arguments[0];
                    return true;
                default:
                    indexExpression = default(Expression);
                    return false;
            }
        }

        /// <summary>
        /// Detemrines if an expression in a default member get
        /// </summary>
        /// <param name="methodCallExp"></param>
        /// <returns></returns>
        public static bool IsDefaultMemberGet(MethodCallExpression methodCallExp)
        {
            var defaultMembers = TypeInfoShim.GetDefaultMembers(methodCallExp.Object.Type.GetTypeInfo());
            //check method call has the same name as the get default member
            //have to check on names as some cases the Methods are the same yet not equal
            return defaultMembers.OfType<PropertyInfo>().Any(pi => pi.GetMethod.Name == methodCallExp.Method.Name);
        }

        /// <summary>
        /// Attempts to evaluate the expression locally to produce a result.
        /// </summary>
        /// <typeparam name="T">the result type.</typeparam>
        /// <param name="exp">The expression to evaluate locally.</param>
        /// <param name="result">The output of the expression</param>
        /// <returns><c>true</c> if expression was evaluated; otherwise, <c>false</c>.</returns>
        public static bool TryEvaluate<T>(Expression exp, out T result)
        {
            return ExpressionEvaluator.TryEvaluate(exp, out result);
        }

        /// <summary>
        /// Attemps to read a property path defined in an expression in the order it appears
        /// typically a property path is represented outside-in e.g. (((x).Author).Name) is Name->Author->x
        /// while this method will read the path inside-out  e.g. (((x).Author).Name) as x->Author->Name
        /// </summary>
        /// <param name="exp">The expression to read the property path</param>
        /// <param name="propertyPath">Each property access in order of parameter to final property</param>
        /// <returns><c>true</c> if path could be determined; otherwise, <c>false</c>.</returns>
        public static bool TryReadPropertyPath(Expression exp, out IEnumerable<Expression> propertyPath)
        {
            return PropertyPathReader.TryReadPropertyPath(exp, out propertyPath);
        }

        /// <summary>
        /// Tries to parse a path in the form 'x.Authors[3].FirstName' into a lambda expression selecting the same property
        /// </summary>
        /// <param name="modelRoot">The type of the intiial parameter in the path</param>
        /// <param name="path">A stirng path to the property to be selected</param>
        /// <param name="expression">The lambda expression selecting the property</param>
        /// <param name="throwOnError">If <c>true</c> an exception is thrown rather than a return value</param>
        /// <returns><c>true</c> if path could be parsed; otherwise, <c>false</c>.</returns>
        public static bool TryParsePath(Type modelRoot, string path, out LambdaExpression expression, bool throwOnError)
        {
            expression = null;
            if (modelRoot == null)
                return throwOnError ? throw new ArgumentNullException(nameof(modelRoot)) : false;
            if (string.IsNullOrWhiteSpace(path))
                return throwOnError ? throw new ArgumentException(nameof(path)) : false;

            var pathParts = path.Split('.');
            var parameterRegex = new Regex(@"(?:^(?<parameter>\w+))");
            var pathPartRegex = new Regex(string.Join("|", new[]
                {
                @"(?:\.(?<property>\w+))",
                @"(?:\[(?<intIndex>\d+)\])",
                @"(?:\['(?<stringIndex>\w+)'\])",
                @"(?:\[""(?<stringIndex>\w+)""\])",
            }));

            var parameterMatch = parameterRegex.Match(path);
            if (!parameterMatch.Success)
                return throwOnError ? throw new Exception($"{nameof(path)} '{path}' must begin with a parameter e.g. 'x.Author.Name'") : false;

            var paramExp = Expression.Parameter(modelRoot, parameterMatch.Groups["parameter"].Value);
            Expression exp = paramExp;

            foreach (var pathPartMatch in pathPartRegex.Matches(path).Cast<Match>())
            {
                var property = pathPartMatch.Groups["property"];
                if (property.Success)
                {
                    var propInfo = TypeInfoShim.GetProperty(exp.Type.GetTypeInfo(), property.Value);
                    exp = Expression.MakeMemberAccess(exp, propInfo);
                    continue;
                }

                var intIndex = pathPartMatch.Groups["intIndex"];
                if (intIndex.Success)
                {
                    var indexProperty = TypeInfoShim.GetDefaultMembers(exp.Type.GetTypeInfo())
                        .OfType<PropertyInfo>()
                        .FirstOrDefault(x => x.SetMethod.GetParameters().Length == 2);
                    if (indexProperty == null)
                        return throwOnError ? throw new Exception($"Unable to find default index member for type '{exp.Type.GetTypeInfo()}'") : false;
                    exp = Expression.MakeIndex(exp, indexProperty, new[] { Expression.Constant(int.Parse(intIndex.Value)) });
                    continue;
                }

                var stringIndex = pathPartMatch.Groups["stringIndex"];
                if (stringIndex.Success)
                {
                    var indexProperty = TypeInfoShim.GetDefaultMembers(exp.Type.GetTypeInfo())
                        .OfType<PropertyInfo>()
                        .FirstOrDefault(x => x.SetMethod.GetParameters().Length == 2);
                    if (indexProperty == null)
                        return throwOnError ? throw new Exception($"Unable to find default index member for type '{exp.Type.GetTypeInfo()}'") : false;
                    exp = Expression.MakeIndex(exp, indexProperty, new[] { Expression.Constant(stringIndex.Value) });
                    continue;
                }
            }
            expression = Expression.Lambda(exp, paramExp);
            return true;
        }
    }
}
