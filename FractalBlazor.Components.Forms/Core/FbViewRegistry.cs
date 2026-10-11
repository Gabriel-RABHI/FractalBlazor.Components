using System.Collections.Concurrent;
using System.Reflection;
using FractalBlazor.Components.Common.Attributes;
using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Mapping;
using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Forms.Core
{
    /// <summary>
    /// Registered as a singleton. Explicit registrations come first; otherwise the loaded
    /// assemblies are scanned once each for <see cref="ViewForAttribute{TSelector}"/>.
    /// A view declared for a base type also displays the derived types.
    /// </summary>
    public class FbViewRegistry<TSelector> : IFbViewRegistry<TSelector>
        where TSelector : struct, Enum
    {
        private readonly ConcurrentDictionary<(Type Model, TSelector Selector), Type> _registry = new();
        private readonly ConcurrentDictionary<(Type Model, TSelector Selector), Type?> _resolved = new();
        private readonly HashSet<Assembly> _scannedAssemblies = new();
        private readonly object _scanLock = new();

        public void Register<TModel, TComponent>(TSelector selector) where TComponent : IComponent
        {
            _registry[(typeof(TModel), selector)] = typeof(TComponent);
            _resolved.Clear();
        }

        public Type? GetViewType(Type modelType, TSelector selector)
        {
            ArgumentNullException.ThrowIfNull(modelType);
            return _resolved.GetOrAdd((modelType, selector), static (key, registry) => registry.Resolve(key.Model, key.Selector), this);
        }

        private Type? Resolve(Type modelType, TSelector selector)
        {
            ScanLoadedAssemblies();

            for (var type = modelType; type is not null; type = type.BaseType)
            {
                if (_registry.TryGetValue((type, selector), out var view))
                    return view;
            }

            // Simple values need no declared view: strings, enumerations, numbers, dates…
            return FbValueView.Supports(modelType) ? typeof(FbValueView) : null;
        }

        private void ScanLoadedAssemblies()
        {
            lock (_scanLock)
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (!assembly.IsDynamic && _scannedAssemblies.Add(assembly))
                        ScanAssembly(assembly);
                }
            }
        }

        private void ScanAssembly(Assembly assembly)
        {
            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException exception)
            {
                // Keep the types that did load: system assemblies often fail on a few.
                types = exception.Types.Where(type => type is not null).ToArray()!;
            }

            foreach (var type in types)
            {
                if (!type.IsClass || type.IsAbstract || !typeof(IComponent).IsAssignableFrom(type))
                    continue;

                foreach (var attribute in type.GetCustomAttributes<ViewForAttribute<TSelector>>())
                    _registry.TryAdd((attribute.ModelType, attribute.Selector), type);
            }
        }
    }
}
