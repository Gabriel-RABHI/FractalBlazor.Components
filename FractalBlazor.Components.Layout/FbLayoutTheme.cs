using System.Text;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Layout
{
    public sealed class FbLayoutTheme : IFbCssVariables
    {
        public string BackgroundAnchor { get; set; } = "#111113";

        public string BackgroundTint { get; set; } = "#34343A";

        public string BackgroundHighAnchor { get; set; } = "#F7F7F8";

        [System.Obsolete("Use BackgroundHighAnchor instead.")]
        public string ForegroundAnchor
        {
            get => BackgroundHighAnchor;
            set => BackgroundHighAnchor = value;
        }

        public string SurfaceMix { get; set; } = "8%";

        public string AccentOffset { get; set; } = "10%";

        public string HighlightOffset { get; set; } = "18%";

        public string FrameLightMix { get; set; } = "8%";

        public string FrameMediumMix { get; set; } = "14%";

        public string FrameStrongMix { get; set; } = "22%";

        public string FrameLightSize { get; set; } = "0.0625rem";

        public string FrameMediumSize { get; set; } = "0.0625rem";

        public string FrameStrongSize { get; set; } = "0.125rem";

        public string SpaceS { get; set; } = FbLayoutPresets.ToRem(FbLayoutPresets.S);

        public string SpaceM { get; set; } = FbLayoutPresets.ToRem(FbLayoutPresets.M);

        public string SpaceL { get; set; } = FbLayoutPresets.ToRem(FbLayoutPresets.L);

        public string SpaceX { get; set; } = FbLayoutPresets.ToRem(FbLayoutPresets.X);

        public string RadiusS { get; set; } = FbLayoutPresets.ToRem(FbLayoutPresets.RS);

        public string RadiusM { get; set; } = FbLayoutPresets.ToRem(FbLayoutPresets.RM);

        public string RadiusL { get; set; } = FbLayoutPresets.ToRem(FbLayoutPresets.RL);

        public string RadiusX { get; set; } = FbLayoutPresets.ToRem(FbLayoutPresets.RX);

        public static FbLayoutTheme Dark() => new();

        public static FbLayoutTheme Light() => new()
        {
            BackgroundAnchor = "#F7F7F8",
            BackgroundTint = "#D8DAE0",
            BackgroundHighAnchor = "#111113",
            SurfaceMix = "8%",
            AccentOffset = "8%",
            HighlightOffset = "16%"
        };

        public string ToCssVariables()
        {
            var builder = new StringBuilder();

            Append(builder, "--fb-default-bg-anchor", BackgroundAnchor);
            Append(builder, "--fb-default-bg-tint", BackgroundTint);
            Append(builder, "--fb-default-bg-high-anchor", BackgroundHighAnchor);
            Append(builder, "--fb-default-bg-surface-mix", SurfaceMix);
            Append(builder, "--fb-default-bg-accent-offset", AccentOffset);
            Append(builder, "--fb-default-bg-highlight-offset", HighlightOffset);
            Append(builder, "--fb-default-frame-light-mix", FrameLightMix);
            Append(builder, "--fb-default-frame-medium-mix", FrameMediumMix);
            Append(builder, "--fb-default-frame-strong-mix", FrameStrongMix);
            Append(builder, "--fb-default-frame-light-size", FrameLightSize);
            Append(builder, "--fb-default-frame-medium-size", FrameMediumSize);
            Append(builder, "--fb-default-frame-strong-size", FrameStrongSize);
            Append(builder, "--fb-default-space-s", SpaceS);
            Append(builder, "--fb-default-space-m", SpaceM);
            Append(builder, "--fb-default-space-l", SpaceL);
            Append(builder, "--fb-default-space-x", SpaceX);
            Append(builder, "--fb-default-radius-s", RadiusS);
            Append(builder, "--fb-default-radius-m", RadiusM);
            Append(builder, "--fb-default-radius-l", RadiusL);
            Append(builder, "--fb-default-radius-x", RadiusX);
            AppendRaw(builder, "--fb-bg-anchor:var(--fb-default-bg-anchor);");
            AppendRaw(builder, "--fb-bg-tint:var(--fb-default-bg-tint);");
            AppendRaw(builder, "--fb-bg-high-anchor:var(--fb-default-bg-high-anchor);");
            AppendRaw(builder, "--fb-bg-surface-mix:var(--fb-default-bg-surface-mix);");
            AppendRaw(builder, "--fb-bg-accent-offset:var(--fb-default-bg-accent-offset);");
            AppendRaw(builder, "--fb-bg-highlight-offset:var(--fb-default-bg-highlight-offset);");
            AppendRaw(builder, "--fb-frame-light-mix:var(--fb-default-frame-light-mix);");
            AppendRaw(builder, "--fb-frame-medium-mix:var(--fb-default-frame-medium-mix);");
            AppendRaw(builder, "--fb-frame-strong-mix:var(--fb-default-frame-strong-mix);");
            AppendRaw(builder, "--fb-frame-light-size:var(--fb-default-frame-light-size);");
            AppendRaw(builder, "--fb-frame-medium-size:var(--fb-default-frame-medium-size);");
            AppendRaw(builder, "--fb-frame-strong-size:var(--fb-default-frame-strong-size);");
            AppendRaw(builder, "--fb-space-s:var(--fb-default-space-s);");
            AppendRaw(builder, "--fb-space-m:var(--fb-default-space-m);");
            AppendRaw(builder, "--fb-space-l:var(--fb-default-space-l);");
            AppendRaw(builder, "--fb-space-x:var(--fb-default-space-x);");
            AppendRaw(builder, "--fb-radius-s:var(--fb-default-radius-s);");
            AppendRaw(builder, "--fb-radius-m:var(--fb-default-radius-m);");
            AppendRaw(builder, "--fb-radius-l:var(--fb-default-radius-l);");
            AppendRaw(builder, "--fb-radius-x:var(--fb-default-radius-x);");

            return builder.ToString();
        }

        private static void Append(StringBuilder builder, string name, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                builder.Append(name).Append(':').Append(value).Append(';');
        }

        private static void AppendRaw(StringBuilder builder, string value)
            => builder.Append(value);
    }

    public class FbLayoutThemeScope : FbComponentBase
    {
        [Parameter]
        public FbLayoutTheme Theme { get; set; } = FbLayoutTheme.Dark();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public string Classes { get; set; } = "";

        [Parameter]
        public string Style { get; set; } = "";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", $"fb-theme-scope {Classes}".TrimEnd());
            builder.AddAttribute(2, "style", $"{Theme.ToCssVariables()}{Style}");
            builder.AddContent(3, ChildContent);
            builder.CloseElement();
        }
    }

    public class FbLayoutThemeStyle : FbComponentBase
    {
        [Parameter]
        public FbLayoutTheme Theme { get; set; } = FbLayoutTheme.Dark();

        [Parameter]
        public string Selector { get; set; } = ":root";

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "style");
            builder.AddContent(1, $"{Selector}{{{Theme.ToCssVariables()}}}");
            builder.CloseElement();
        }
    }
}
