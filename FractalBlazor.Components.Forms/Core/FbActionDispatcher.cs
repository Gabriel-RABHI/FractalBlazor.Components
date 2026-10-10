using System.Collections.Concurrent;
using FractalBlazor.Components.Forms.Contracts;
using System.Linq.Expressions;
using System.Reflection;

namespace FractalBlazor.Components.Forms.Core
{
    /// <summary>
    /// Sends an action whose type is only known at run time (a button's <c>Action</c>) to the
    /// matching <c>Handle</c> method of a state. Each (state type, action type) pair is
    /// compiled once.
    /// </summary>
    public static class FbActionDispatcher
    {
        private static readonly ConcurrentDictionary<(Type Handler, Type Action), Action<object, object>> _handlers = new();

        public static void Dispatch(object handlerInstance, IStateAction action)
        {
            ArgumentNullException.ThrowIfNull(handlerInstance);
            ArgumentNullException.ThrowIfNull(action);

            var handler = _handlers.GetOrAdd((handlerInstance.GetType(), action.GetType()), static key => Compile(key.Handler, key.Action));
            handler(handlerInstance, action);
        }

        private static Action<object, object> Compile(Type handlerType, Type actionType)
        {
            var method = handlerType.GetMethod("Handle", BindingFlags.Public | BindingFlags.Instance, null, [actionType], null)
                ?? throw new MissingMethodException(
                    $"Type '{handlerType.Name}' is missing a public method: void Handle({actionType.Name} action)");

            var instance = Expression.Parameter(typeof(object), "instance");
            var action = Expression.Parameter(typeof(object), "action");
            var call = Expression.Call(Expression.Convert(instance, handlerType), method, Expression.Convert(action, actionType));

            return Expression.Lambda<Action<object, object>>(call, instance, action).Compile();
        }
    }

    public static class FbActionDispatcher<TAction>
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
