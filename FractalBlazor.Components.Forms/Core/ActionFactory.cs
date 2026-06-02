using System.Linq.Expressions;

namespace FractalBlazor.Components.Forms.Core
{
    public static class ActionFactory<TAction, TValue>
    {
        // This delegate is cached permanently in memory for this specific TAction
        public static readonly Func<TValue, TAction> Create;

        static ActionFactory()
        {
            var valueType = typeof(TValue);
            var actionType = typeof(TAction);

            // 1. Find the constructor that takes TValue (e.g., string)
            var ctor = actionType.GetConstructor(new[] { valueType });

            if (ctor == null)
            {
                throw new InvalidOperationException(
                    $"Type {actionType.Name} must have a constructor that accepts a single {valueType.Name} parameter.");
            }

            // 2. Define the input parameter for the delegate
            var parameterExp = Expression.Parameter(valueType, "newValue");

            // 3. Create the 'new TAction(newValue)' expression
            var newExp = Expression.New(ctor, parameterExp);

            // 4. Compile it into a high-performance Func delegate
            Create = Expression.Lambda<Func<TValue, TAction>>(newExp, parameterExp).Compile();
        }
    }
}
