using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;

namespace FractalBlazor.Components.Forms.Core
{
    public static class ActionDispatcher<TAction>
    {
        private static readonly ConcurrentDictionary<Type, Action<object, TAction>> _handlers = new();

        public static void Dispatch(object handlerInstance, TAction action)
        {
            var handlerType = handlerInstance.GetType();

            if (_handlers.TryGetValue(handlerType, out var compiledDelegate))
            {
                compiledDelegate(handlerInstance, action);
            } else
            {
                compiledDelegate = CompileDelegate(handlerType);
                _handlers.TryAdd(handlerType, compiledDelegate);
                compiledDelegate(handlerInstance, action);
            }
        }

        private static Action<object, TAction> CompileDelegate(Type targetType)
        {
            var actionType = typeof(TAction);

            var methodInfo = targetType.GetMethod("Handle",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { actionType },
                null);

            if (methodInfo == null)
            {
                throw new MissingMethodException(
                    $"Type '{targetType.Name}' is missing a public method: void Handle({actionType.Name} action)");
            }

            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var actionParam = Expression.Parameter(typeof(TAction), "action");

            var castInstance = Expression.Convert(instanceParam, targetType);
            var callExpression = Expression.Call(castInstance, methodInfo, actionParam);

            return Expression.Lambda<Action<object, TAction>>(
                callExpression, instanceParam, actionParam).Compile();
        }
    }
}
