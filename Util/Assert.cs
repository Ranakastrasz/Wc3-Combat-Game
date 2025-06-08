using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Util
{
    internal class AssertUtil
    {
        public static void Assert(Expression<Func<bool>> conditionExpr, string? customMessage = null, [CallerMemberName] string? callerName = null)
        {
            if (!conditionExpr.Compile()())
            {
                string conditionText = conditionExpr.Body.ToString();
                string message = customMessage == null
                    ? $"Assertion failed in {callerName}: {conditionText}"
                    : $"Assertion failed in {callerName}: {conditionText} - {customMessage}";
                throw new InvalidOperationException(message);
            }
        }
        public static void AssertNotNull<T>(
        [NotNull] T? value,
        [CallerArgumentExpression("value")] string? paramName = null)
        where T : class
        {
            if (value is null)
                throw new InvalidOperationException($"Argument '{paramName}' must not be null.");
        }
    }
}
