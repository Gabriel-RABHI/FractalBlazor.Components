using FractalBlazor.Components.Layout.Abstracts;
using FractalBlazor.Components.Layout.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace FractalBlazor.Components.Layout
{
    /// <summary>
    /// A panel that slides from an edge (<c>Left</c>, <c>Right</c>, <c>Top</c>, <c>Bottom</c>),
    /// controlled by <see cref="IsOpen"/>: the drawer never closes itself. Escape, a click on
    /// the backdrop or any close request of the browser only raise <see cref="IsOpenChanged"/>
    /// with <c>false</c>, and the state decides. The surface flags (<c>WBS</c>, <c>WFL</c>,
    /// <c>PL</c>, <c>WRM</c>…) style the panel, filled with the surface by default.
    /// </summary>
    /// <remarks>
    /// Three modes (<see cref="FbDrawerMode"/>): <c>Modal</c> opens a native dialog on top of
    /// the whole screen, and renders its content only while open; <c>Inline</c> takes its
    /// place in the layout and pushes it; <c>InlineModal</c> takes its place but covers its
    /// zone, the nearest positioned ancestor (a row or a stack). A closed inline drawer keeps
    /// its content, made inert.
    /// </remarks>
    public class FbDrawer : FbLayoutSurfaceComponentBase, IHandleAfterRender, IDisposable
    {
        private static readonly string[] SideNames = ["left", "right", "top", "bottom"];
        private static readonly string[] ModeNames = ["modal", "inline", "inline-modal"];

        // Frame classes of every side, mode and state, built once.
        private static readonly string[] FrameClasses = BuildFrameClasses();

        private FbDrawerSide _side = FbDrawerSide.Left;
        private FbDrawerMode _mode = FbDrawerMode.Modal;
        private (FbDrawerSide Side, FbDrawerMode Mode) _defaults;
        private ElementReference _dialog;
        private DotNetObjectReference<FbDrawer>? _self;
        private bool _showing;
        private bool _disposed;

        // Last free-form values and the strings built from them.
        private string? _size;
        private string? _sizeStyle;
        private string? _dialogFrame;
        private string? _dialogPanel;
        private string? _dialogClasses;

        public FbDrawer()
        {
            WBS = true;
        }

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        /// <summary>
        /// Whether the drawer is open. Only the state changes it.
        /// </summary>
        [Parameter]
        public bool IsOpen { get; set; }

        /// <summary>
        /// Raised with <c>false</c> when the user asks to close the drawer.
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Width of a side drawer, height of a top or bottom one (<c>18rem</c> by default).
        /// </summary>
        [Parameter]
        public string? Size { get; set; }

        /// <summary>
        /// Accessible name of the drawer.
        /// </summary>
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public FbDrawerSide Side { get => _side; set => _side = value; }

        [Parameter]
        public bool Left { get => _side == FbDrawerSide.Left; set { if (value) _side = FbDrawerSide.Left; } }

        [Parameter]
        public bool Right { get => _side == FbDrawerSide.Right; set { if (value) _side = FbDrawerSide.Right; } }

        [Parameter]
        public bool Top { get => _side == FbDrawerSide.Top; set { if (value) _side = FbDrawerSide.Top; } }

        [Parameter]
        public bool Bottom { get => _side == FbDrawerSide.Bottom; set { if (value) _side = FbDrawerSide.Bottom; } }

        [Parameter]
        public FbDrawerMode Mode { get => _mode; set => _mode = value; }

        [Parameter]
        public bool Modal { get => _mode == FbDrawerMode.Modal; set { if (value) _mode = FbDrawerMode.Modal; } }

        [Parameter]
        public bool Inline { get => _mode == FbDrawerMode.Inline; set { if (value) _mode = FbDrawerMode.Inline; } }

        [Parameter]
        public bool InlineModal { get => _mode == FbDrawerMode.InlineModal; set { if (value) _mode = FbDrawerMode.InlineModal; } }

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            _defaults = (_side, _mode);
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            (_side, _mode) = _defaults;
        }

        /// <summary>
        /// Called by the browser when the user asks to close a modal drawer: Escape, a click
        /// on the backdrop, or another close request. The dialog stays open until the state
        /// closes it.
        /// </summary>
        [JSInvokable]
        public Task OnDismiss() => DismissAsync();

        private Task DismissAsync()
            => IsOpen && !_disposed && IsOpenChanged.HasDelegate ? IsOpenChanged.InvokeAsync(false) : Task.CompletedTask;

        private Task HandleKeyDownAsync(KeyboardEventArgs args)
            => args.Key == "Escape" ? DismissAsync() : Task.CompletedTask;

        #region HIDDEN
        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
        private struct PanelKey
        {
            public SurfaceKey Surface;
        }

        private string FrameClass => FrameClasses[((int)_side * ModeNames.Length + (int)_mode) * 2 + (IsOpen ? 1 : 0)];

        private string PanelClasses
            => ResolveClasses(new PanelKey { Surface = SurfaceKeyValue },
                static component => ((FbDrawer)component).BuildPanelClasses());

        private string BuildPanelClasses()
        {
            var line = new FbAtomLine();
            AddSurfaceAtoms(line);
            return line.Classes(FbClasses.Join("fb-drawer-panel", BuildSurfaceClasses()), null);
        }

        /// <summary>
        /// Classes of the dialog of a modal drawer, which is both the frame and the panel.
        /// </summary>
        private string DialogClasses
        {
            get
            {
                var frame = FrameClass;
                var panel = PanelClasses;
                if (!ReferenceEquals(frame, _dialogFrame) || !ReferenceEquals(panel, _dialogPanel))
                {
                    _dialogFrame = frame;
                    _dialogPanel = panel;
                    _dialogClasses = frame + " " + panel;
                }
                return _dialogClasses!;
            }
        }

        private string? SizeStyle
        {
            get
            {
                if (!string.Equals(Size, _size, StringComparison.Ordinal))
                {
                    _size = Size;
                    _sizeStyle = string.IsNullOrWhiteSpace(Size) ? null : $"--fb-drawer-size:{Size.Trim()}";
                }
                return _sizeStyle;
            }
        }

        private static string[] BuildFrameClasses()
        {
            var classes = new string[SideNames.Length * ModeNames.Length * 2];
            for (var side = 0; side < SideNames.Length; side++)
            {
                for (var mode = 0; mode < ModeNames.Length; mode++)
                {
                    var frame = $"fb-drawer fb-drawer-{ModeNames[mode]} fb-drawer-{SideNames[side]}";
                    var index = (side * ModeNames.Length + mode) * 2;
                    classes[index] = frame;
                    classes[index + 1] = frame + " fb-drawer-open";
                }
            }
            return classes;
        }
        #endregion

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (_mode == FbDrawerMode.Modal)
            {
                // The dialog is the panel. It stays in the page, empty while closed, and the
                // script shows it once its content is rendered.
                builder.OpenElement(0, "dialog");
                builder.AddAttribute(1, "class", DialogClasses);
                builder.AddAttribute(2, "style", FbInlineStyle.Join(SizeStyle, AggregatedStyles));
                builder.AddAttribute(3, "aria-label", Title);
                builder.AddElementReferenceCapture(4, reference => _dialog = reference);
                if (IsOpen || _showing)
                    builder.AddContent(5, ChildContent);
                builder.CloseElement();
                return;
            }

            var overlay = _mode == FbDrawerMode.InlineModal;
            builder.OpenElement(10, "div");
            builder.AddAttribute(11, "class", FrameClass);
            builder.AddAttribute(12, "style", SizeStyle);
            builder.AddAttribute(13, "inert", !IsOpen);
            if (overlay)
            {
                builder.AddAttribute(14, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, HandleKeyDownAsync));
                builder.OpenElement(15, "div");
                builder.AddAttribute(16, "class", "fb-drawer-scrim");
                builder.AddAttribute(17, "onclick", EventCallback.Factory.Create(this, DismissAsync));
                builder.CloseElement();
            }
            builder.OpenElement(18, "div");
            builder.AddAttribute(19, "class", PanelClasses);
            builder.AddAttribute(20, "style", AggregatedStyles);
            builder.AddAttribute(21, "role", overlay ? "dialog" : Title is null ? null : "region");
            builder.AddAttribute(22, "aria-label", Title);
            builder.AddContent(23, ChildContent);
            builder.CloseElement();
            builder.CloseElement();
        }

        /// <summary>
        /// Shows the dialog of a modal drawer once it opens, and closes it, after its exit
        /// transition, before its content goes away.
        /// </summary>
        async Task IHandleAfterRender.OnAfterRenderAsync()
        {
            if (_disposed)
                return;

            if (_mode != FbDrawerMode.Modal)
            {
                // The dialog left the page with the mode, and the top layer with it.
                _showing = false;
                return;
            }

            try
            {
                if (IsOpen && !_showing)
                {
                    // Set before the call: a close rendered meanwhile must find it showing.
                    _showing = true;
                    _self ??= DotNetObjectReference.Create(this);
                    await JS.InvokeVoidAsync("FractalBlazor.showDrawer", _dialog, _self);
                }
                else if (!IsOpen && _showing)
                {
                    _showing = false;
                    await JS.InvokeVoidAsync("FractalBlazor.closeDrawer", _dialog);
                    if (!_disposed)
                        StateHasChanged();
                }
            }
            catch (JSDisconnectedException)
            {
                // The circuit is gone: the dialog no longer exists.
            }
        }

        /// <summary>
        /// A removed dialog leaves the top layer by itself: only the reference given to the
        /// script is released.
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            _self?.Dispose();
        }
    }
}
