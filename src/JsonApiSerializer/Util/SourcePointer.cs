using JsonApiSerializer.ContractResolvers;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.JsonConverters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace JsonApiSerializer.Util
{
    public class SourcePointer
    {
        /// <summary>
        /// Utility class to write and encode source pointers
        /// </summary>
        private class SourcePointerBuilder
        {
            private readonly StringBuilder _sb = new StringBuilder();

            public SourcePointerBuilder Append(string value)
            {
                var encoded = value
                    .Replace("~", "~0")
                    .Replace("/", "~1")
                    .Replace(@"\", @"\\")
                    .Replace("\"", "\\");
                _sb.Append("/").Append(encoded);
                return this;
            }

            public SourcePointerBuilder Append(MemberInfo memberInfo, JsonApiSerializerSettings settings)
            {
                var contractResolver = settings.ContractResolver as JsonApiContractResolver;

                //Try and determine what hte json name for the member is, that is the one we want to output
                var declaringContract = contractResolver.ResolveContract(memberInfo.DeclaringType) as JsonObjectContract;
                var jsonProp = declaringContract?.Properties?.FirstOrDefault(x => x.UnderlyingName == memberInfo.Name);

                this.Append(jsonProp?.PropertyName ?? memberInfo.Name);
                return this;
            }

            public override string ToString()
            {
                return _sb.ToString();
            }
        }

        /// <summary>
        /// Determines the equivalent JsonApi source pointer for a given model property
        /// </summary>
        /// <typeparam name="TRoot">The type of the paramter in the model path</typeparam>
        /// <param name="modelPath">The path to the model property. The first parameter must be the parameter e.g. x.Authors[2].FirstName</param>
        /// <param name="settings">The JsonApiSerializerSettings used to during deserialization</param>
        /// <returns>JsonApi source pointer</returns>
        public static string FromModel<TRoot>(string modelPath, JsonApiSerializerSettings settings)
        {
           return FromModel(typeof(TRoot), modelPath, settings);
        }

        /// <summary>
        /// Determines the equivalent JsonApi source pointer for a given model property
        /// </summary>
        /// <param name="modelRoot">The type of the paramter in the model path</param>
        /// <param name="modelPath">The path to the model property. The first parameter must be the parameter e.g. x.Authors[2].FirstName</param>
        /// <param name="settings">The JsonApiSerializerSettings used to during deserialization</param>
        /// <returns>JsonApi source pointer</returns>
        public static string FromModel(Type modelRoot, string modelPath, JsonApiSerializerSettings settings)
        {
            ExpressionUtil.TryParsePath(modelRoot, modelPath, out var modelPathExpression, throwOnError: true);
            SourcePointer.TryFromModel(modelPathExpression, settings, out string sourcePointer, throwOnError: true);
            return sourcePointer;
        }

        /// <summary>
        /// Determines the equivalent JsonApi source pointer for a given model property
        /// </summary>
        /// <typeparam name="TData">The type of the paramter in the model path</typeparam>
        /// <param name="modelExpressionPath">Expression selecting the property</param>
        /// <param name="settings">The JsonApiSerializerSettings used to during deserialization</param>
        /// <returns>JsonApi source pointer</returns>
        public static string FromModel<TData>(Expression<Func<TData, object>> modelExpressionPath, JsonApiSerializerSettings settings)
        {
            var success = SourcePointer.TryFromModel((LambdaExpression)modelExpressionPath, settings, out string sourcePointer, throwOnError: true);
            return sourcePointer;
        }



        /// <summary>
        /// Tries to determine the equivalent JsonApi source pointer for a given model property
        /// </summary>
        /// <param name="modelRoot">The type of the paramter in the model path</param>
        /// <param name="modelPath">Expression selecting property</param>
        /// <param name="settings">The JsonApiSerializerSettings used to during deserialization</param>
        /// <param name="sourcePointer">JsonApi source pointer</param>
        /// <returns><c>true</c> if source pointer could be determined; otherwise, <c>false</c></returns>
        public static bool TryFromModel(Type modelRoot, string modelPath, JsonApiSerializerSettings settings, out string sourcePointer)
        {
            sourcePointer = default(string);
            return ExpressionUtil.TryParsePath(modelRoot, modelPath, out var modelPathExpression, throwOnError: false)
                && SourcePointer.TryFromModel(modelPathExpression, settings, out sourcePointer, throwOnError: false);
        }

        /// <summary>
        /// Tries to determine the equivalent JsonApi source pointer for a given model property
        /// </summary>
        /// <typeparam name="TRoot">The type of the paramter in the model path</typeparam>
        /// <param name="modelPath">The path to the model property. The first parameter must be the parameter e.g. x.Authors[2].FirstName</param>
        /// <param name="settings">The JsonApiSerializerSettings used to during deserialization</param>
        /// <param name="sourcePointer">JsonApi source pointer</param>
        /// <returns><c>true</c> if source pointer could be determined; otherwise, <c>false</c></returns>
        public static bool TryFromModel<TRoot>(string modelPath, JsonApiSerializerSettings settings, out string sourcePointer)
        {
            return TryFromModel(typeof(TRoot), modelPath, settings, out sourcePointer);
        }

        /// <summary>
        /// Tries to determine the equivalent JsonApi source pointer for a given model property
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <param name="modelExpressionPath">Expression selecting the property</param>
        /// <param name="settings">The JsonApiSerializerSettings used to during deserialization</param>
        /// <param name="sourcePointer">JsonApi source pointer</param>
        /// <returns><c>true</c> if source pointer could be determined; otherwise, <c>false</c></returns>
        public static bool TryFromModel<TData>(Expression<Func<TData, object>> modelExpressionPath, JsonApiSerializerSettings settings, out string sourcePointer)
        {
            return SourcePointer.TryFromModel((LambdaExpression)modelExpressionPath, settings, out sourcePointer, throwOnError: false);
        }

        private static bool TryFromModel(LambdaExpression modelExpressionPath, JsonApiSerializerSettings settings, out string jsonApiPath, bool throwOnError)
        {
            jsonApiPath = null;
            if (modelExpressionPath.Parameters.Count != 1)
                return throwOnError ? throw new NotSupportedException("Only single parameter member selection expression can be resolved") : false;

            if (!ExpressionUtil.TryReadPropertyPath(modelExpressionPath.Body, out var expressionList))
                return throwOnError ? throw new Exception($"Unable to process expresion '{modelExpressionPath}' as a property path") : false;

            var pointer = new SourcePointerBuilder();
            var contractResolver = (JsonApiContractResolver)settings.ContractResolver;

            //if we didnt start at a document root, add the 'data' to the path
            var isDocRoot = DocumentRootConverter.CanConvertStatic(modelExpressionPath.Parameters[0].Type);
            if (!isDocRoot)
                pointer.Append("data");

            foreach (var exp in expressionList)
            {
                if(exp is ParameterExpression)
                {
                    //initial parameters are not expression in the jsonApiPath
                    continue;
                }
                else if(ExpressionUtil.IsIndexAccess(exp, out var indexExpression))
                {
                    if (!ExpressionUtil.TryEvaluate<object>(indexExpression, out var index))
                        return throwOnError ? throw new Exception($"Unable to process index expression '{indexExpression}'") : false;

                    pointer.Append(index.ToString());
                }
                else if (exp is MemberExpression memberExpression)
                {
                    var propertyType = memberExpression.Type;
                    var containingType = memberExpression.Expression.Type;


                    if (contractResolver.ResourceObjectConverter.CanConvert(containingType))
                    {

                        if (contractResolver.ResourceObjectListConverter.CanConvert(propertyType)
                            || contractResolver.ResourceObjectConverter.CanConvert(propertyType))
                        {
                            pointer.Append("relationships");
                            pointer.Append(memberExpression.Member, settings);
                            pointer.Append("data");
                        }
                        else if (contractResolver.ResourceRelationshipConverter.CanConvert(propertyType))
                        {
                            pointer.Append("relationships");
                            pointer.Append(memberExpression.Member, settings);
                        }
                        else
                        {
                            pointer.Append("attributes");
                            pointer.Append(memberExpression.Member, settings);
                        }
                    }
                    else
                    {
                        pointer.Append(memberExpression.Member, settings);
                    }
                }
                else
                {
                    return throwOnError ? throw new Exception($"Unknown expression '{exp}'") : false;
                }
            }

            jsonApiPath = pointer.ToString();
            return true;
        }
    }
}
