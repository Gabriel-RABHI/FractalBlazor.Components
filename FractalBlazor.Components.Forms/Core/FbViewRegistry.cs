using FractalBlazor.Components.Forms.Attributes;
using FractalBlazor.Components.Forms.Contracts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FractalBlazor.Components.Forms.Core
{
    public class FbViewRegistry<TSelector> : IFbViewRegistry<TSelector>
        where TSelector : Enum
    {
        private readonly ConcurrentDictionary<Type, Type> _registry = new();
        private readonly HashSet<Assembly> _scannedAssemblies = new();
        private readonly object _scanLock = new();

        public void Register<TModel, TComponent>() where TComponent : IComponent
        {
            _registry.TryAdd(typeof(TModel), typeof(TComponent));
        }

        public Type? GetViewType(Type modelType)
        {
            if (_registry.TryGetValue(modelType, out var viewType))
                return viewType;
            return ScanForMissingView(modelType);
        }

        private Type? ScanForMissingView(Type targetModelType)
        {
            lock (_scanLock)
            {
                if (_registry.TryGetValue(targetModelType, out var viewType))
                    return viewType;

                var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => !a.IsDynamic);

                bool foundNewViews = false;

                foreach (var assembly in loadedAssemblies)
                {
                    if (_scannedAssemblies.Add(assembly))
                    {
                        ScanAssembly(assembly);
                        foundNewViews = true;
                    }
                }

                if (foundNewViews && _registry.TryGetValue(targetModelType, out viewType))
                    return viewType;

                return null;
            }
        }

        private void ScanAssembly(Assembly assembly)
        {
            try
            {
                var viewTypes = assembly.GetTypes()
                    .Where(t => t.IsClass && !t.IsAbstract && typeof(IComponent).IsAssignableFrom(t));

                foreach (var type in viewTypes)
                {
                    var attribute = type.GetCustomAttribute<ViewForAttribute<TSelector>>();
                    if (attribute != null)
                    {
                        _registry.TryAdd(attribute.ModelType, type);
                    }
                }
            } catch (ReflectionTypeLoadException)
            {
                // Safely ignore assemblies that fail to load their types (common with system assemblies)
            }
        }
    }
}
