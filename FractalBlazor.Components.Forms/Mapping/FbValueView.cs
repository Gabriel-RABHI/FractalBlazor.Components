using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using FractalBlazor.Components.Forms.Display;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Mapping
{
    /// <summary>
    /// Selectors of the views that the library itself asks <see cref="FbDynamicView{TSelector}"/> for.
    /// </summary>
    public enum FbViewSelector
    {
        /// <summary>
        /// An element shown in a list: the options of <c>FbSelect</c> and its current value.
        /// </summary>
        Item
    }

    /// <summary>
    /// Built-in view of simple values: strings, enumerations, numbers, dates, durations and
    /// identifiers. The view registry falls back to it, for every selector, when no view is
    /// declared for such a type; a declared view always wins.
    /// </summary>
    public sealed class FbValueView : FbComponentBase
    {
        private static readonly ConcurrentDictionary<Enum, string> _enumTexts = new();

        [Parameter]
        public object? Item { get; set; }

        /// <summary>
        /// True for the types this view displays.
        /// </summary>
        public static bool Supports(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;
            return type == typeof(string) || type.IsEnum || type.IsPrimitive
                || type == typeof(decimal) || type == typeof(DateTime) || type == typeof(DateTimeOffset)
                || type == typeof(DateOnly) || type == typeof(TimeOnly) || type == typeof(TimeSpan)
                || type == typeof(Guid);
        }

        /// <summary>
        /// Text of a simple value: the <c>[Display(Name)]</c> of an enumeration member when it
        /// has one, the current-culture form of a formattable value, its string form otherwise.
        /// </summary>
        public static string TextOf(object? value) => value switch
        {
            null => "",
            string text => text,
            Enum member => _enumTexts.GetOrAdd(member, static member => EnumText(member)),
            IFormattable formattable => formattable.ToString(null, CultureInfo.CurrentCulture),
            _ => value.ToString() ?? ""
        };

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent<FbText>(0);
            builder.AddAttribute(1, nameof(FbText.Value), TextOf(Item));
            builder.CloseComponent();
        }

        private static string EnumText(Enum member)
        {
            var name = member.ToString();
            var field = member.GetType().GetField(name, BindingFlags.Public | BindingFlags.Static);
            return field?.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? name;
        }
    }
}
