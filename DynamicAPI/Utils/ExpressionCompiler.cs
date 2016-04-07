using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CSharp;

namespace Igorious.StardewValley.DynamicAPI.Utils
{
    public static class ExpressionCompiler
    {
        #region Classes

        private class ParameterInfo
        {
            public string Name { get; set; }
            public Type Type { get; set; }
        }

        #endregion

        #region Private Data

        private static IDictionary<string, object> CachedFunctions { get; } = new Dictionary<string, object>();

        #endregion

        #region	Public Methods

        public static TFunc CompileExpression<TFunc>(string body, params string[] args)
        {
            if (string.IsNullOrWhiteSpace(body)) return default(TFunc);

            var key = $"{string.Join(", ", args)} => {body}";
            object value;
            if (CachedFunctions.TryGetValue(key, out value)) return (TFunc)value;

            var genericArgs = typeof(TFunc).GetGenericArguments();
            var argTypes = genericArgs.Take(genericArgs.Length - 1);
            var resultType = genericArgs.Last();

            var methodInfo = CompileMethod(resultType, argTypes.Select((a, i) => new ParameterInfo {Name = args[i], Type = a}), body);
            var expression = (TFunc)(object)Delegate.CreateDelegate(typeof(TFunc), methodInfo);
            CachedFunctions.Add(key, expression);
            return expression;
        }

        #endregion

        #region	Auxiliary Methods

        private static MethodInfo CompileMethod(Type result, IEnumerable<ParameterInfo> parameters, string body)
        {
            var code = @"             
            public static class DynamicFunction
            {                
                public static $TypeResult Function($Arguments)
                {
                    return $Body;
                }
            }"
            .Replace("$TypeResult", result.FullName)
            .Replace("$Arguments", string.Join(", ", parameters.Select(p => $"{p.Type.FullName} {p.Name}")))
            .Replace("$Body", body);

            var provider = new CSharpCodeProvider();
            var results = provider.CompileAssemblyFromSource(new CompilerParameters() {GenerateInMemory = true }, code);
            if (results.Errors.HasErrors) return null;

            var dynamicFunction = results.CompiledAssembly.GetType("DynamicFunction");
            return dynamicFunction.GetMethod("Function");
        }

        #endregion
    }
}