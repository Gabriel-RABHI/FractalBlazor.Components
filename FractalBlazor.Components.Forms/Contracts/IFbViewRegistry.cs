using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Forms.Contracts
{
    /// <summary>
    /// Maps a state type and a selector (<c>Viewer</c>, <c>Summary</c>, <c>Editor</c>…) to the
    /// component that displays it. Views declare themselves with <c>[ViewFor&lt;TSelector&gt;]</c>
    /// or are registered explicitly. Simple values (strings, enumerations, numbers, dates…)
    /// fall back to the built-in <c>FbValueView</c> when no view is declared for them.
    /// </summary>
    public interface IFbViewRegistry<TSelector>
        where TSelector : struct, Enum
    {
        void Register<TModel, TComponent>(TSelector selector) where TComponent : IComponent;

        /// <summary>
        /// The view for <paramref name="modelType"/>, or for its closest base type, with the
        /// given selector; <c>null</c> when none is known.
        /// </summary>
        Type? GetViewType(Type modelType, TSelector selector);
    }
}
