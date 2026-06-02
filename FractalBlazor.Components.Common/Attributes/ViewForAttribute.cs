namespace FractalBlazor.Components.Forms.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ViewForAttribute : Attribute
    {
        public Type ModelType { get; }

        public ViewForAttribute(Type modelType) => ModelType = modelType;
    }
}
