namespace FractalBlazor.Components.Common.Attributes
{
    /// <summary>
    /// Declares that a component displays <see cref="ModelType"/> (and its derived types)
    /// for <see cref="Selector"/>. A component can carry several of these attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class ViewForAttribute<TSelector> : Attribute
        where TSelector : struct, Enum
    {
        public Type ModelType { get; }

        public TSelector Selector { get; }

        public ViewForAttribute(Type modelType, TSelector selector)
        {
            ModelType = modelType;
            Selector = selector;
        }
    }
}
