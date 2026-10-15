using System.Globalization;
using FractalBlazor.Components.Forms.Theming.Helpers;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Forms.Components.Display
{
    /// <summary>
    /// Shows how far a task has gone: a bar, horizontal or <see cref="Vertical"/>, or a
    /// <see cref="Circular"/> ring. Without a <see cref="Value"/>, it is indeterminate and moves
    /// until a value comes.
    /// </summary>
    /// <remarks>
    /// A bar fills from its start (the left, or the bottom when vertical), a ring from the top,
    /// clockwise. The track takes the medium frame color of its background, the filled part
    /// the accent of the variant: a <see cref="Variant"/> (<c>Success</c>, <c>Error</c>…) or
    /// an enclosing <c>FbVariant</c> colors it. The element is an ARIA <c>progressbar</c>.
    /// </remarks>
    /// <example><c>&lt;FbProgress Value="@Item.Done" Max="@Item.Total" VL /&gt;</c></example>
    public sealed class FbProgress : FbComponentBase
    {
        private Flags _flags, _defaultFlags;

        private struct Flags
        {
            public bool Circular, Vertical, VL;
        }

        /// <summary>
        /// How far the task has gone, from 0 to <see cref="Max"/>; <c>null</c> while it cannot
        /// be told, which makes the progress indeterminate.
        /// </summary>
        [Parameter]
        public double? Value { get; set; }

        /// <summary>
        /// The value of a finished task (100 by default).
        /// </summary>
        [Parameter]
        public double Max { get; set; } = 100;

        /// <summary>
        /// A ring instead of a bar, filled from the top, clockwise.
        /// </summary>
        [Parameter]
        public bool Circular { get => _flags.Circular; set => _flags.Circular = value; }

        /// <summary>
        /// A bar filled from the bottom up. It takes the height of its parent, 8rem at least.
        /// </summary>
        [Parameter]
        public bool Vertical { get => _flags.Vertical; set => _flags.Vertical = value; }

        /// <summary>
        /// Value -> Label: the share done, at the end of the bar or in the middle of the ring.
        /// </summary>
        [Parameter]
        public bool VL { get => _flags.VL; set => _flags.VL = value; }

        /// <summary>
        /// A .NET format for the share done, between 0 and 1: <c>P0</c> by default, <c>P1</c>…
        /// </summary>
        [Parameter]
        public string? Format { get; set; }

        /// <summary>
        /// Shown in place of the label: in the middle of the ring, or at the end of the bar.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Accessible name of the progress.
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        /// <summary>
        /// The diameter of a ring (3rem by default), the length of a bar (the width, or the
        /// height when vertical; all the room by default).
        /// </summary>
        [Parameter]
        public string? Size { get; set; }

        /// <summary>
        /// The thickness of a bar (0.5rem by default), rounded to whole pixels. A ring keeps
        /// its proportions.
        /// </summary>
        [Parameter]
        public string? Thickness { get; set; }

        /// <summary>
        /// Variant of the progress: <c>Success</c>, <c>Warning</c>, <c>Error</c>…
        /// </summary>
        [Parameter]
        public string? Variant { get; set; }

        /// <summary>
        /// Custom CSS classes added to the root element.
        /// </summary>
        [Parameter]
        public string Classes { get; set; } = "";

        /// <summary>
        /// The share done, from 0 to 100, or <c>null</c> while indeterminate.
        /// </summary>
        internal double? Percent
        {
            get
            {
                if (Value is not { } value)
                    return null;
                var percent = Max > 0 ? value / Max * 100 : 0;
                return double.IsFinite(percent) ? Math.Clamp(percent, 0, 100) : 0;
            }
        }

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            _defaultFlags = _flags;
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            _flags = _defaultFlags;
        }

        private string RootClassNames(bool indeterminate)
        {
            var classes = "fb-progress";
            if (_flags.Circular) classes += " fb-progress-circular";
            else if (_flags.Vertical) classes += " fb-progress-vertical";
            if (indeterminate) classes += " fb-progress-indeterminate";
            if (!string.IsNullOrWhiteSpace(Classes)) classes += " " + Classes.Trim();
            return FbVariantScope.WithClasses(classes, FbVariantScope.Classes(FbVariantScope.Effective(Variant, false)));
        }

        private string? RootStyle(double? percent)
        {
            var style = percent is { } value ? "--fb-progress-value:" + Invariant(value) + ";" : "";
            if (!string.IsNullOrWhiteSpace(Size)) style += "--fb-progress-size:" + Size + ";";
            if (!string.IsNullOrWhiteSpace(Thickness)) style += "--fb-progress-thickness:" + Thickness + ";";
            return style.Length == 0 ? null : style;
        }

        private static string Invariant(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

        /// <summary>
        /// The bar, or the ring, in the root that carries the share done and the ARIA values;
        /// then the label.
        /// </summary>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var percent = Percent;
            var label = percent is { } done ? (done / 100).ToString(Format ?? "P0", CultureInfo.CurrentCulture) : null;

            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", RootClassNames(percent is null));
            builder.AddAttribute(2, "style", RootStyle(percent));
            builder.AddAttribute(3, "role", "progressbar");
            builder.AddAttribute(4, "aria-label", Title);
            builder.AddAttribute(5, "aria-valuemin", "0");
            builder.AddAttribute(6, "aria-valuemax", Invariant(Max));
            builder.AddAttribute(7, "aria-valuenow", Value is { } value ? Invariant(Math.Clamp(value, 0, Math.Max(Max, 0))) : null);
            builder.AddAttribute(8, "aria-valuetext", label);

            if (_flags.Circular)
            {
                builder.OpenElement(9, "svg");
                builder.AddAttribute(10, "class", "fb-progress-ring");
                builder.AddAttribute(11, "viewBox", "0 0 100 100");
                builder.AddAttribute(12, "aria-hidden", "true");
                Circle(builder, 13, "fb-progress-ring-track");
                Circle(builder, 20, "fb-progress-ring-fill");
                builder.CloseElement();
            }
            else
            {
                builder.OpenElement(30, "div");
                builder.AddAttribute(31, "class", "fb-progress-track");
                builder.OpenElement(32, "div");
                builder.AddAttribute(33, "class", "fb-progress-fill");
                builder.CloseElement();
                builder.CloseElement();
            }

            if (ChildContent is not null || (_flags.VL && label is not null))
            {
                builder.OpenElement(40, "span");
                builder.AddAttribute(41, "class", "fb-progress-label");
                if (ChildContent is not null)
                    builder.AddContent(42, ChildContent);
                else
                    builder.AddContent(43, label);
                builder.CloseElement();
            }

            builder.CloseElement();
        }

        // A circle of the ring: its length counts as 100, so that the dash of the filled part
        // is the share done.
        private static void Circle(RenderTreeBuilder builder, int sequence, string kind)
        {
            builder.OpenElement(sequence, "circle");
            builder.AddAttribute(sequence + 1, "class", kind);
            builder.AddAttribute(sequence + 2, "cx", "50");
            builder.AddAttribute(sequence + 3, "cy", "50");
            builder.AddAttribute(sequence + 4, "r", "45");
            builder.AddAttribute(sequence + 5, "pathLength", "100");
            builder.CloseElement();
        }
    }
}
