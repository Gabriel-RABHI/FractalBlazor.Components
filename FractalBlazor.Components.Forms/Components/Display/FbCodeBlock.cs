using FractalBlazor.Components.Forms.Components.Actions;
using FractalBlazor.Components.Forms.Components.Icons;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace FractalBlazor.Components.Forms.Components.Display
{
    /// <summary>
    /// Source code in the code font, colored by its syntax, with a button that copies it. The
    /// language is <see cref="Razor"/> by default (whose lines without markup are read as C#),
    /// <see cref="CSharp"/>, <see cref="Css"/> or <see cref="Plain"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The block is a surface: the accent fill and a medium radius by default, and every flag of
    /// the Layout grammar to change them; <c>WRN</c> squares it to close a frame, whose corners
    /// it takes in a <c>Joined</c> group. The code scrolls sideways rather than wrapping.
    /// </para>
    /// <para>
    /// The colors come from the palette of the theme, through the <c>--fb-code-*</c> custom
    /// properties, readable on a dark theme and on a light one. The copy button shows when the
    /// block is hovered or holds the focus; it copies <see cref="Code"/> through the script of
    /// Layout, then says so for a moment.
    /// </para>
    /// </remarks>
    /// <example><c>&lt;FbCodeBlock Code="@Sample" CSharp /&gt;</c></example>
    public sealed class FbCodeBlock : FbLayoutSurfaceComponentBase, IDisposable
    {
        private static readonly string[] TokenClasses =
        [
            "", "fb-code-k", "fb-code-t", "fb-code-f", "fb-code-s", "fb-code-n", "fb-code-c",
            "fb-code-p", "fb-code-g", "fb-code-a", "fb-code-r", "fb-code-y", "fb-code-l", "fb-code-v"
        ];

        private static readonly TimeSpan CopiedFor = TimeSpan.FromSeconds(2);

        private readonly EventCallback<MouseEventArgs> _copy;
        private FbCodeLanguage _language, _defaultLanguage;
        private bool _noCopy, _defaultNoCopy;
        private string? _tokenizedCode;
        private FbCodeLanguage _tokenizedLanguage;
        private IReadOnlyList<FbCodeToken> _tokens = [];
        private bool _copied, _disposed;
        private int _copies;

        public FbCodeBlock()
        {
            WBA = true;
            WRM = true;
            _copy = EventCallback.Factory.Create<MouseEventArgs>(this, CopyAsync);
        }

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        /// <summary>
        /// The code shown, as it is written: Razor never parses it.
        /// </summary>
        [Parameter, EditorRequired]
        public string Code { get; set; } = "";

        /// <summary>
        /// The language of <see cref="Code"/>; the flags <see cref="Razor"/>,
        /// <see cref="CSharp"/>, <see cref="Css"/> and <see cref="Plain"/> set it too.
        /// </summary>
        [Parameter]
        public FbCodeLanguage Language { get => _language; set => _language = value; }

        /// <summary>
        /// Razor, the default: tags, components, attributes and transitions; C# elsewhere.
        /// </summary>
        [Parameter]
        public bool Razor { get => _language == FbCodeLanguage.Razor; set { if (value) _language = FbCodeLanguage.Razor; } }

        /// <summary>
        /// C#: keywords, types, calls, strings, numbers and comments.
        /// </summary>
        [Parameter]
        public bool CSharp { get => _language == FbCodeLanguage.CSharp; set { if (value) _language = FbCodeLanguage.CSharp; } }

        /// <summary>
        /// CSS: selectors, properties, custom properties, functions and values.
        /// </summary>
        [Parameter]
        public bool Css { get => _language == FbCodeLanguage.Css; set { if (value) _language = FbCodeLanguage.Css; } }

        /// <summary>
        /// Plain text, without colors.
        /// </summary>
        [Parameter]
        public bool Plain { get => _language == FbCodeLanguage.Plain; set { if (value) _language = FbCodeLanguage.Plain; } }

        /// <summary>
        /// No copy button.
        /// </summary>
        [Parameter]
        public bool NoCopy { get => _noCopy; set => _noCopy = value; }

        /// <summary>
        /// Accessible name and tooltip of the copy button.
        /// </summary>
        [Parameter]
        public string CopyTitle { get; set; } = "Copy";

        /// <summary>
        /// What the button says, and what is announced, once the code is copied.
        /// </summary>
        [Parameter]
        public string CopiedTitle { get; set; } = "Copied";

        protected override void CaptureParameterDefaults()
        {
            base.CaptureParameterDefaults();
            (_defaultLanguage, _defaultNoCopy) = (_language, _noCopy);
        }

        protected override void RestoreParameterDefaults()
        {
            base.RestoreParameterDefaults();
            (_language, _noCopy) = (_defaultLanguage, _defaultNoCopy);
        }

        /// <summary>
        /// The pieces of the code, cut again only when the code or its language changes.
        /// </summary>
        internal IReadOnlyList<FbCodeToken> Tokens
        {
            get
            {
                if (!ReferenceEquals(_tokenizedCode, Code) || _tokenizedLanguage != _language)
                {
                    (_tokenizedCode, _tokenizedLanguage) = (Code, _language);
                    _tokens = FbCodeTokenizer.Tokenize(Code, _language);
                }
                return _tokens;
            }
        }

        /// <summary>
        /// The block, its code, and the copy button with the live region that announces a copy.
        /// </summary>
        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenElement(0, "div");
            builder.AddAttribute(1, "class", _copied ? "fb-code-block fb-code-copied " + AggregatedClasses : "fb-code-block " + AggregatedClasses);
            builder.AddAttribute(2, "style", AggregatedStyles);

            builder.OpenElement(3, "pre");
            builder.AddAttribute(4, "class", "fb-code-pre");
            builder.AddAttribute(5, "tabindex", "0");
            builder.OpenElement(6, "code");
            foreach (var token in Tokens)
            {
                if (token.Kind == FbCodeTokenKind.Plain)
                {
                    builder.AddContent(7, token.Text);
                    continue;
                }
                builder.OpenElement(8, "span");
                builder.AddAttribute(9, "class", TokenClasses[(int)token.Kind]);
                builder.AddContent(10, token.Text);
                builder.CloseElement();
            }
            builder.CloseElement();
            builder.CloseElement();

            if (!_noCopy)
            {
                builder.OpenElement(11, "div");
                builder.AddAttribute(12, "class", "fb-code-copy");
                builder.OpenComponent<FbButton>(13);
                builder.AddComponentParameter(14, nameof(FbButton.KS), true);
                builder.AddComponentParameter(15, nameof(FbButton.S), true);
                builder.AddComponentParameter(16, nameof(FbButton.Title), _copied ? CopiedTitle : CopyTitle);
                builder.AddComponentParameter(17, nameof(FbButton.OnClick), _copy);
                builder.AddComponentParameter(18, nameof(FbButton.ChildContent), _copied ? CopiedIcon : CopyIcon);
                builder.CloseComponent();
                builder.CloseElement();

                builder.OpenElement(19, "span");
                builder.AddAttribute(20, "class", "fb-code-status");
                builder.AddAttribute(21, "role", "status");
                builder.AddContent(22, _copied ? CopiedTitle : null);
                builder.CloseElement();
            }
            builder.CloseElement();
        }

        private static readonly RenderFragment CopyIcon = Icon(FbMaterialIcons.ContentCopy);

        private static readonly RenderFragment CopiedIcon = Icon(FbMaterialIcons.Check);

        private static RenderFragment Icon(string icon) => builder =>
        {
            builder.OpenComponent<FbIcon>(0);
            builder.AddComponentParameter(1, nameof(FbIcon.Icon), icon);
            builder.AddComponentParameter(2, nameof(FbIcon.S), true);
            builder.CloseComponent();
        };

        /// <summary>
        /// Copies the code, then says so for <see cref="CopiedFor"/>. A copy the browser refuses
        /// says nothing.
        /// </summary>
        private async Task CopyAsync()
        {
            bool copied;
            try
            {
                copied = await JS.InvokeAsync<bool>("FractalBlazor.copyText", Code);
            }
            catch (JSDisconnectedException)
            {
                return;
            }
            catch (JSException)
            {
                copied = false;
            }
            if (!copied || _disposed)
                return;

            var copy = ++_copies;
            _copied = true;
            Render();
            await Task.Delay(CopiedFor);
            if (_disposed || copy != _copies)
                return;
            _copied = false;
            Render();
        }

        public void Dispose() => _disposed = true;
    }
}
