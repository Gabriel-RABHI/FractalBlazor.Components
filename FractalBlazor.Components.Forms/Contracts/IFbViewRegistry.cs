using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace FractalBlazor.Components.Forms.Contracts
{

    public interface IFbViewRegistry
    {
        void Register<TModel, TComponent>() where TComponent : IComponent;

        Type? GetViewType(Type modelType);
    }
}
