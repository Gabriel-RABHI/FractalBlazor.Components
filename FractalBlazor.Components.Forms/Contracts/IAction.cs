using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Contracts
{
    public interface IStateAction<TValue>
    {
        // A standard record automatically implements this property
        TValue NewValue { get; }
    }

    public interface IViewRegistry
    {
        void Register<TModel, TComponent>() where TComponent : IComponent;

        Type? GetViewType(Type modelType);
    }
}
