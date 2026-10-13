using System.Reflection;
using FractalBlazor.Components.Forms.Components.Actions;
using FractalBlazor.Components.Layout;
using FractalBlazor.WebSite.StateModel;
using Microsoft.AspNetCore.Components;

namespace FractalBlazor.WebSite.Flags;

/// <summary>
/// The flags of FractalBlazor, read on the components themselves: every public component of
/// Layout and Forms, and each of its boolean parameters. <see cref="FlagGrammar"/> says what
/// the letters of a name stand for, the reflection which components accept it. Built once:
/// the catalog never changes while the application runs.
/// </summary>
public sealed class ComponentFlagCatalog : IFlagCatalog
{
    // Parameters that hold a state rather than a flag written in the markup.
    private static readonly HashSet<string> NotFlags = ["IsOpen"];

    public ComponentFlagCatalog()
        : this([typeof(FbRow).Assembly, typeof(FbButton).Assembly])
    {
    }

    public ComponentFlagCatalog(IEnumerable<Assembly> assemblies)
    {
        var grammar = FlagGrammar.Build();
        var flags = new Dictionary<(string Name, string Meaning), (FlagGrammar.Reading Reading, SortedSet<string> Components)>();
        var components = new SortedSet<string>(StringComparer.Ordinal);
        var unexplained = new SortedSet<string>(StringComparer.Ordinal);

        foreach (var type in assemblies.SelectMany(assembly => assembly.GetExportedTypes()).Where(IsComponent))
        {
            var component = DisplayName(type);
            foreach (var property in FlagProperties(type))
            {
                var family = DisplayName(property.DeclaringType!);
                if (!grammar.TryRead(family, property.Name, out var reading))
                {
                    unexplained.Add($"{family}.{property.Name}");
                    reading = FlagGrammar.Unexplained(property.Name);
                }

                var key = (property.Name, string.Join(" → ", reading.Tokens.Select(token => token.Meaning)));
                if (!flags.TryGetValue(key, out var entry))
                    flags[key] = entry = (reading, new SortedSet<string>(StringComparer.Ordinal));
                entry.Components.Add(component);
                components.Add(component);
            }
        }

        Flags = flags.Values
            .OrderBy(entry => entry.Reading.Order)
            .ThenBy(entry => entry.Reading.Tokens[0].Code, StringComparer.Ordinal)
            .Select(entry => new FlagInfo(entry.Reading.Name, entry.Reading.Tokens, entry.Components.ToList()))
            .ToList();
        Components = components.ToList();
        Unexplained = unexplained.ToList();
    }

    public IReadOnlyList<FlagInfo> Flags { get; }

    public IReadOnlyList<string> Components { get; }

    /// <summary>
    /// Flags the grammar does not explain, as <c>Family.Name</c>: listed with their name
    /// alone. A test keeps it empty.
    /// </summary>
    public IReadOnlyList<string> Unexplained { get; }

    private static bool IsComponent(Type type)
        => type is { IsClass: true, IsAbstract: false, IsNested: false } && typeof(IComponent).IsAssignableFrom(type);

    /// <summary>
    /// The boolean parameters of <paramref name="type"/>, each name once: a parameter
    /// redeclared with <c>new</c> hides the one of its base.
    /// </summary>
    private static IEnumerable<PropertyInfo> FlagProperties(Type type)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        for (var current = type; current is not null && current != typeof(object); current = current.BaseType)
        {
            var declared = current.IsGenericType && !current.IsGenericTypeDefinition ? current.GetGenericTypeDefinition() : current;
            foreach (var property in declared.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                if (property.PropertyType != typeof(bool) || property.GetCustomAttribute<ParameterAttribute>() is null)
                    continue;
                if (NotFlags.Contains(property.Name) || !seen.Add(property.Name))
                    continue;
                yield return property;
            }
        }
    }

    /// <summary>
    /// The name of a type as written in the markup: <c>FbSelect</c> for <c>FbSelect`1</c>.
    /// </summary>
    private static string DisplayName(Type type)
    {
        var tick = type.Name.IndexOf('`');
        return tick < 0 ? type.Name : type.Name[..tick];
    }
}
