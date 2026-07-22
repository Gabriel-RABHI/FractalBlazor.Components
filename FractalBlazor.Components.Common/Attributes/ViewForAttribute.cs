namespace FractalBlazor.Components.Forms.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ViewForAttribute<TSelector> : Attribute
        where TSelector : Enum
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
