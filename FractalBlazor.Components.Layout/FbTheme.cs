using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public interface IFbCssVariables
    {
        string ToCssVariables();
    }

    public static class FbTheme
    {
        public static readonly IFbCssVariables Default = ColorVariable("default");
        public static readonly IFbCssVariables Disabled = ColorVariable("disabled");
        public static readonly IFbCssVariables Focused = ColorVariable("focused");
        public static readonly IFbCssVariables Error = ColorVariable("error");
        public static readonly IFbCssVariables Success = ColorVariable("success");
        public static readonly IFbCssVariables Selected = ColorVariable("selected");

        [Obsolete("Use Selected instead.")]
        public static IFbCssVariables Sellected => Selected;

        public static IFbCssVariables ColorVariable(string name)
            => new FbColorVariableReference(name);

        public static FbColorPalette ColorPalette(string name)
            => new(name);

        public static IFbCssVariables Combine(params IFbCssVariables[] variables)
            => new FbCssVariablesSet(variables);
    }

    public sealed class FbCssVariablesSet : IFbCssVariables
    {
        private readonly IReadOnlyList<IFbCssVariables> _variables;

        public FbCssVariablesSet(IReadOnlyList<IFbCssVariables> variables)
        {
            _variables = variables;
        }

        public string ToCssVariables()
        {
            var builder = new StringBuilder();

            foreach (var variables in _variables)
                builder.Append(variables?.ToCssVariables());

            return builder.ToString();
        }
    }

    public sealed class FbColorVariableReference : IFbCssVariables
    {
        public FbColorVariableReference(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "default" : name.Trim();
        }

        public string Name { get; }

        public string ToCssVariables()
        {
            var prefix = $"--fb-{Name}-";

            return
                $"--fb-bg-anchor:var({prefix}bg-anchor,var(--fb-default-bg-anchor));" +
                $"--fb-bg-high-anchor:var({prefix}bg-high-anchor,var(--fb-default-bg-high-anchor));" +
                $"--fb-bg-accent-offset:var({prefix}bg-accent-offset,var(--fb-default-bg-accent-offset));" +
                $"--fb-bg-highlight-offset:var({prefix}bg-highlight-offset,var(--fb-default-bg-highlight-offset));" +
                $"--fb-frame-light-mix:var({prefix}frame-light-mix,var(--fb-default-frame-light-mix));" +
                $"--fb-frame-medium-mix:var({prefix}frame-medium-mix,var(--fb-default-frame-medium-mix));" +
                $"--fb-frame-strong-mix:var({prefix}frame-strong-mix,var(--fb-default-frame-strong-mix));" +
                $"--fb-fg-anchor:var({prefix}fg-anchor,var(--fb-default-fg-anchor));" +
                $"--fb-fg-high-anchor:var({prefix}fg-high-anchor,var(--fb-default-fg-high-anchor));" +
                $"--fb-fg-default-high-mix:var({prefix}fg-default-high-mix,var(--fb-default-fg-default-high-mix));" +
                $"--fb-fg-subtle-high-mix:var({prefix}fg-subtle-high-mix,var(--fb-default-fg-subtle-high-mix));" +
                $"--fb-fg-muted-high-mix:var({prefix}fg-muted-high-mix,var(--fb-default-fg-muted-high-mix));" +
                $"--fb-fg-highlight-high-mix:var({prefix}fg-highlight-high-mix,var(--fb-default-fg-highlight-high-mix));";
        }
    }

    public sealed class FbColorPalette : IFbCssVariables
    {
        public FbColorPalette(string name)
        {
            Name = string.IsNullOrWhiteSpace(name) ? "default" : name.Trim();
        }

        public string Name { get; }

        public string BackgroundAnchor { get; set; } = "";

        public string BackgroundHighAnchor { get; set; } = "";

        public string BackgroundAccentOffset { get; set; } = "";

        public string BackgroundHighlightOffset { get; set; } = "";

        public string FrameLightMix { get; set; } = "";

        public string FrameMediumMix { get; set; } = "";

        public string FrameStrongMix { get; set; } = "";

        public string ForegroundAnchor { get; set; } = "";

        public string ForegroundHighAnchor { get; set; } = "";

        public string ForegroundDefaultHighMix { get; set; } = "";

        public string ForegroundSubtleHighMix { get; set; } = "";

        public string ForegroundMutedHighMix { get; set; } = "";

        public string ForegroundHighlightHighMix { get; set; } = "";

        public string ToCssVariables()
        {
            var builder = new StringBuilder();
            var prefix = $"--fb-{Name}-";

            Append(builder, $"{prefix}bg-anchor", BackgroundAnchor);
            Append(builder, $"{prefix}bg-high-anchor", BackgroundHighAnchor);
            Append(builder, $"{prefix}bg-accent-offset", BackgroundAccentOffset);
            Append(builder, $"{prefix}bg-highlight-offset", BackgroundHighlightOffset);
            Append(builder, $"{prefix}frame-light-mix", FrameLightMix);
            Append(builder, $"{prefix}frame-medium-mix", FrameMediumMix);
            Append(builder, $"{prefix}frame-strong-mix", FrameStrongMix);
            Append(builder, $"{prefix}fg-anchor", ForegroundAnchor);
            Append(builder, $"{prefix}fg-high-anchor", ForegroundHighAnchor);
            Append(builder, $"{prefix}fg-default-high-mix", ForegroundDefaultHighMix);
            Append(builder, $"{prefix}fg-subtle-high-mix", ForegroundSubtleHighMix);
            Append(builder, $"{prefix}fg-muted-high-mix", ForegroundMutedHighMix);
            Append(builder, $"{prefix}fg-highlight-high-mix", ForegroundHighlightHighMix);

            return builder.ToString();
        }

        private static void Append(StringBuilder builder, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                builder.Append(name).Append(':').Append(value).Append(';');
        }
    }

    public class FbThemeStyle : FbComponentBase
    {
        [Parameter]
        public IFbCssVariables? Variables { get; set; }

        [Parameter]
        public IReadOnlyList<IFbCssVariables>? VariableSets { get; set; }

        [Parameter]
        public string Selector { get; set; } = ":root";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "style");
            builder.AddContent(1, $"{Selector}{{{ToCssVariables()}}}");
            builder.CloseElement();
        }

        private string ToCssVariables()
        {
            var builder = new StringBuilder();

            builder.Append(Variables?.ToCssVariables());

            if (VariableSets is not null)
            {
                foreach (var variables in VariableSets)
                    builder.Append(variables?.ToCssVariables());
            }

            return builder.ToString();
        }
    }

    public class FbThemeScope : FbComponentBase
    {
        [Parameter]
        public IFbCssVariables? Variables { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public string Classes { get; set; } = "";

        [Parameter]
        public string Style { get; set; } = "";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", $"fb-theme-scope {Classes}".TrimEnd());
            builder.AddAttribute(2, "style", $"{Variables?.ToCssVariables()}{Style}");
            builder.AddContent(3, ChildContent);
            builder.CloseElement();
        }
    }
}
