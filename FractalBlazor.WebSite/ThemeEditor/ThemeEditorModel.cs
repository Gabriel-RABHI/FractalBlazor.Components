using System.Globalization;
using System.Text;
using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Forms.Theming.Solver;
using FractalBlazor.Components.Layout.Colors;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.WebSite.ThemeEditor
{
    /// <summary>
    /// Values edited from the default theme: the spacings, corners and typography of the
    /// theme, and the mixes and anchors of one branch. Every change bumps
    /// <see cref="Version"/>; <see cref="Setup"/> is rebuilt only then, so the preview theme is
    /// resolved again only when something was really edited.
    /// </summary>
    public sealed class ThemeEditorModel
    {
        private readonly IFbFormThemeRegistry _registry;
        private FbResolvedTheme _original = default!;
        private FbThemeSetup? _setup;
        private int _setupVersion = -1;

        public ThemeEditorModel(IFbFormThemeRegistry registry)
        {
            _registry = registry;
            Load(FbThemeBranches.Dark);
            ResetShape();
            ResetTypography();
        }

        public static IReadOnlyList<string> Branches { get; } = [FbThemeBranches.Dark, FbThemeBranches.Light];

        public int Version { get; private set; }

        public string Branch { get; private set; } = FbThemeBranches.Dark;

        public string SelectedVariant { get; set; } = FbThemeVariants.Default;

        public IReadOnlyList<string> VariantNames => _original.VariantOrder;

        // -------- State of the editor itself, kept across navigation
        public int EditTab { get; set; }

        public int PreviewTab { get; set; }

        /// <summary>
        /// The color copied from an anchor, ready to be pasted into another one.
        /// </summary>
        public string? CopiedColor { get; set; }

        // -------- Mixes of the branch, in percent
        public double SurfaceMix { get => _surfaceMix; set => Set(ref _surfaceMix, value); }
        public double AccentOffset { get => _accentOffset; set => Set(ref _accentOffset, value); }
        public double HighlightOffset { get => _highlightOffset; set => Set(ref _highlightOffset, value); }
        public double HoverOffset { get => _hoverOffset; set => Set(ref _hoverOffset, value); }
        public double FrameLight { get => _frameLight; set => Set(ref _frameLight, value); }
        public double FrameMedium { get => _frameMedium; set => Set(ref _frameMedium, value); }
        public double FrameStrong { get => _frameStrong; set => Set(ref _frameStrong, value); }
        public double TextSubtle { get => _textSubtle; set => Set(ref _textSubtle, value); }
        public double TextMuted { get => _textMuted; set => Set(ref _textMuted, value); }
        public double TextDefault { get => _textDefault; set => Set(ref _textDefault, value); }
        public double TextHighlight { get => _textHighlight; set => Set(ref _textHighlight, value); }

        private double _surfaceMix, _accentOffset, _highlightOffset, _hoverOffset, _frameLight, _frameMedium, _frameStrong,
            _textSubtle, _textMuted, _textDefault, _textHighlight;

        // -------- Spacings and corners, in rem: values of the theme, shared by its branches
        public double SpaceS { get => _spaceS; set => Set(ref _spaceS, value); }
        public double SpaceM { get => _spaceM; set => Set(ref _spaceM, value); }
        public double SpaceL { get => _spaceL; set => Set(ref _spaceL, value); }
        public double SpaceX { get => _spaceX; set => Set(ref _spaceX, value); }
        public double RadiusS { get => _radiusS; set => Set(ref _radiusS, value); }
        public double RadiusM { get => _radiusM; set => Set(ref _radiusM, value); }
        public double RadiusL { get => _radiusL; set => Set(ref _radiusL, value); }
        public double RadiusX { get => _radiusX; set => Set(ref _radiusX, value); }

        private double _spaceS, _spaceM, _spaceL, _spaceX, _radiusS, _radiusM, _radiusL, _radiusX;

        // -------- Typography: values of the theme. Sizes in px, coefficients and weights bare.
        /// <summary>
        /// A CSS font-family list. A value the theme would refuse (<c>;</c>, braces…) is ignored.
        /// </summary>
        public string TextFontFamily { get => _textFont; set => SetCss(ref _textFont, value); }
        public string CodeFontFamily { get => _codeFont; set => SetCss(ref _codeFont, value); }
        public double FontSize { get => _fontSize; set => Set(ref _fontSize, value); }
        public double LineHeight { get => _lineHeight; set => Set(ref _lineHeight, value); }
        public double CoefXS { get => _coefXS; set => Set(ref _coefXS, value); }
        public double CoefS { get => _coefS; set => Set(ref _coefS, value); }
        public double CoefM { get => _coefM; set => Set(ref _coefM, value); }
        public double CoefL { get => _coefL; set => Set(ref _coefL, value); }
        public double CoefXL { get => _coefXL; set => Set(ref _coefXL, value); }
        public double CoefXXL { get => _coefXXL; set => Set(ref _coefXXL, value); }
        public double WeightThin { get => _weightThin; set => Set(ref _weightThin, value); }
        public double WeightDefault { get => _weightDefault; set => Set(ref _weightDefault, value); }
        public double WeightBold { get => _weightBold; set => Set(ref _weightBold, value); }
        public double WeightExtraBold { get => _weightExtraBold; set => Set(ref _weightExtraBold, value); }

        private string _textFont = "", _codeFont = "";
        private double _fontSize, _lineHeight, _coefXS, _coefS, _coefM, _coefL, _coefXL, _coefXXL,
            _weightThin, _weightDefault, _weightBold, _weightExtraBold;

        private readonly Dictionary<string, Anchors> _anchors = new(StringComparer.OrdinalIgnoreCase);

        public Anchors Selected => _anchors[SelectedVariant];

        /// <summary>
        /// Theme built from the edited values, inheriting from the registered default theme.
        /// </summary>
        public FbThemeSetup Setup
        {
            get
            {
                if (_setup is null || _setupVersion != Version)
                {
                    _setup = BuildSetup();
                    _setupVersion = Version;
                }
                return _setup;
            }
        }

        /// <summary>
        /// Starts again from the spacings and corners of the default theme. They belong to the
        /// theme, so <see cref="Load"/> keeps them.
        /// </summary>
        public void ResetShape()
        {
            _spaceS = Rem(_original.Spacings.S);
            _spaceM = Rem(_original.Spacings.M);
            _spaceL = Rem(_original.Spacings.L);
            _spaceX = Rem(_original.Spacings.X);
            _radiusS = Rem(_original.Corners.S);
            _radiusM = Rem(_original.Corners.M);
            _radiusL = Rem(_original.Corners.L);
            _radiusX = Rem(_original.Corners.X);
            Version++;
        }

        /// <summary>
        /// Starts again from the typography of the default theme, kept by <see cref="Load"/>.
        /// </summary>
        public void ResetTypography()
        {
            var typography = _original.Typography;
            _textFont = typography.TextFontFamily!;
            _codeFont = typography.CodeFontFamily!;
            _fontSize = Px(typography.FontSizeBase);
            _lineHeight = Number(typography.LineHeight);
            _coefXS = Number(typography.ExtraSmallCoef);
            _coefS = Number(typography.SmallCoef);
            _coefM = Number(typography.MediumCoef);
            _coefL = Number(typography.LargeCoef);
            _coefXL = Number(typography.ExtraLargeCoef);
            _coefXXL = Number(typography.ExtraExtraLargeCoef);
            _weightThin = Number(typography.ThinWeight);
            _weightDefault = Number(typography.DefaultWeight);
            _weightBold = Number(typography.BoldWeight);
            _weightExtraBold = Number(typography.ExtraBoldWeight);
            Version++;
        }

        /// <summary>
        /// Starts again from the resolved values of <paramref name="branch"/>: its mixes and
        /// anchors. The spacings, corners and typography of the theme are kept.
        /// </summary>
        public void Load(string branch)
        {
            Branch = branch;
            _original = _registry.Resolve(_registry.Default.Name, branch);

            _surfaceMix = Percent(_original.SurfaceMix.SurfaceMix);
            _accentOffset = Percent(_original.SurfaceMix.AccentOffset);
            _highlightOffset = Percent(_original.SurfaceMix.HighlightOffset);
            _hoverOffset = Percent(_original.SurfaceMix.HoverOffset);
            _frameLight = Percent(_original.BordersMix.LightMix);
            _frameMedium = Percent(_original.BordersMix.MediumMix);
            _frameStrong = Percent(_original.BordersMix.StrongMix);
            _textSubtle = Percent(_original.TextVariant.SubtleHighMix);
            _textMuted = Percent(_original.TextVariant.MutedHighMix);
            _textDefault = Percent(_original.TextVariant.DefaultHighMix);
            _textHighlight = Percent(_original.TextVariant.HighlightHighMix);

            _anchors.Clear();
            foreach (var name in _original.VariantOrder)
            {
                var variant = _original.GetVariant(name);
                _anchors[name] = new Anchors(this,
                    variant.LayoutColors.LowAnchor!, variant.LayoutColors.HighAnchor!,
                    variant.FormColors.LowAnchor!, variant.FormColors.HighAnchor!,
                    variant.FrameColors.LowAnchor, variant.FrameColors.HighAnchor);
            }

            Version++;
        }

        /// <summary>
        /// C# of a theme that declares only what differs from the default theme.
        /// </summary>
        public string ToCSharp()
        {
            const string ThemeIndent = "    ", BranchIndent = "            ";
            var typography = _original.Typography;

            var theme = new StringBuilder();
            AppendGroup(theme, ThemeIndent, "Spacings", "FbThemeLayoutSpacings",
                Value("S", CssRem(_spaceS), CssRem(Rem(_original.Spacings.S))),
                Value("M", CssRem(_spaceM), CssRem(Rem(_original.Spacings.M))),
                Value("L", CssRem(_spaceL), CssRem(Rem(_original.Spacings.L))),
                Value("X", CssRem(_spaceX), CssRem(Rem(_original.Spacings.X))));
            AppendGroup(theme, ThemeIndent, "Corners", "FbThemeLayoutCorners",
                Value("S", CssRem(_radiusS), CssRem(Rem(_original.Corners.S))),
                Value("M", CssRem(_radiusM), CssRem(Rem(_original.Corners.M))),
                Value("L", CssRem(_radiusL), CssRem(Rem(_original.Corners.L))),
                Value("X", CssRem(_radiusX), CssRem(Rem(_original.Corners.X))));
            AppendGroup(theme, ThemeIndent, "Typography", "FbThemeFormTypography",
                Value("TextFontFamily", _textFont, typography.TextFontFamily),
                Value("CodeFontFamily", _codeFont, typography.CodeFontFamily),
                Value("FontSizeBase", CssPx(_fontSize), CssPx(Px(typography.FontSizeBase))),
                Value("LineHeight", CssNumber(_lineHeight), CssNumber(Number(typography.LineHeight))),
                Value("ExtraSmallCoef", CssNumber(_coefXS), CssNumber(Number(typography.ExtraSmallCoef))),
                Value("SmallCoef", CssNumber(_coefS), CssNumber(Number(typography.SmallCoef))),
                Value("MediumCoef", CssNumber(_coefM), CssNumber(Number(typography.MediumCoef))),
                Value("LargeCoef", CssNumber(_coefL), CssNumber(Number(typography.LargeCoef))),
                Value("ExtraLargeCoef", CssNumber(_coefXL), CssNumber(Number(typography.ExtraLargeCoef))),
                Value("ExtraExtraLargeCoef", CssNumber(_coefXXL), CssNumber(Number(typography.ExtraExtraLargeCoef))),
                Value("ThinWeight", CssNumber(_weightThin), CssNumber(Number(typography.ThinWeight))),
                Value("DefaultWeight", CssNumber(_weightDefault), CssNumber(Number(typography.DefaultWeight))),
                Value("BoldWeight", CssNumber(_weightBold), CssNumber(Number(typography.BoldWeight))),
                Value("ExtraBoldWeight", CssNumber(_weightExtraBold), CssNumber(Number(typography.ExtraBoldWeight))));

            var branch = new StringBuilder();
            AppendGroup(branch, BranchIndent, "SurfaceMix", "FbThemeLayoutSurfaceMix",
                MixValue("SurfaceMix", _surfaceMix, _original.SurfaceMix.SurfaceMix),
                MixValue("AccentOffset", _accentOffset, _original.SurfaceMix.AccentOffset),
                MixValue("HighlightOffset", _highlightOffset, _original.SurfaceMix.HighlightOffset),
                MixValue("HoverOffset", _hoverOffset, _original.SurfaceMix.HoverOffset));
            AppendGroup(branch, BranchIndent, "BordersMix", "FbThemeLayoutBordersMix",
                MixValue("LightMix", _frameLight, _original.BordersMix.LightMix),
                MixValue("MediumMix", _frameMedium, _original.BordersMix.MediumMix),
                MixValue("StrongMix", _frameStrong, _original.BordersMix.StrongMix));
            AppendGroup(branch, BranchIndent, "TextVariantMix", "FbThemeFormTextVariantMix",
                MixValue("SubtleHighMix", _textSubtle, _original.TextVariant.SubtleHighMix),
                MixValue("MutedHighMix", _textMuted, _original.TextVariant.MutedHighMix),
                MixValue("DefaultHighMix", _textDefault, _original.TextVariant.DefaultHighMix),
                MixValue("HighlightHighMix", _textHighlight, _original.TextVariant.HighlightHighMix));

            var variants = _original.VariantOrder.Where(name => _anchors[name].Changed).ToList();
            if (variants.Count > 0)
            {
                branch.AppendLine($"{BranchIndent}Variants =");
                branch.AppendLine($"{BranchIndent}[");
                for (var i = 0; i < variants.Count; i++)
                {
                    var a = _anchors[variants[i]];
                    var colors = new List<string>
                    {
                        $"LayoutColors = new FbThemeLayoutColors {{ LowAnchor = \"{a.BgLow}\", HighAnchor = \"{a.BgHigh}\" }}",
                        $"FormColors = new FbThemeFormColors {{ LowAnchor = \"{a.FgLow}\", HighAnchor = \"{a.FgHigh}\" }}"
                    };
                    if (a.HasFrameColors)
                        colors.Add($"FrameColors = new FbThemeFormColors {{ LowAnchor = \"{a.FrameLow}\", HighAnchor = \"{a.FrameHigh}\" }}");

                    branch.AppendLine($"{BranchIndent}    new FbThemeColorVariant(\"{variants[i]}\")");
                    branch.AppendLine($"{BranchIndent}    {{");
                    branch.AppendLine(string.Join(",\n", colors.Select(line => $"{BranchIndent}        {line}")));
                    branch.AppendLine(i < variants.Count - 1 ? $"{BranchIndent}    }}," : $"{BranchIndent}    }}");
                }
                branch.AppendLine($"{BranchIndent}]");
            }

            if (theme.Length == 0 && branch.Length == 0)
                return "// Aucune valeur modifiée : le thème par défaut s'applique tel quel.";

            var body = new StringBuilder(theme.ToString());
            if (branch.Length > 0)
            {
                body.AppendLine("    Branches =");
                body.AppendLine("    [");
                body.AppendLine($"        new FbThemeBranch(FbThemeBranches.{Branch})");
                body.AppendLine("        {");
                body.Append(branch.ToString().TrimEnd().TrimEnd(',')).AppendLine();
                body.AppendLine("        }");
                body.AppendLine("    ]");
            }

            var code = new StringBuilder();
            code.AppendLine("var myTheme = new FbThemeSetup(\"MyTheme\", registry.Default)");
            code.AppendLine("{");
            code.Append(body.ToString().TrimEnd().TrimEnd(',')).AppendLine();
            code.Append("};");
            return code.ToString();
        }

        private FbThemeSetup BuildSetup() => new("Edited", _registry.Default)
        {
            Spacings = new FbThemeLayoutSpacings
            {
                S = CssRem(_spaceS),
                M = CssRem(_spaceM),
                L = CssRem(_spaceL),
                X = CssRem(_spaceX)
            },
            Corners = new FbThemeLayoutCorners
            {
                S = CssRem(_radiusS),
                M = CssRem(_radiusM),
                L = CssRem(_radiusL),
                X = CssRem(_radiusX)
            },
            Typography = new FbThemeFormTypography
            {
                TextFontFamily = _textFont,
                CodeFontFamily = _codeFont,
                FontSizeBase = CssPx(_fontSize),
                LineHeight = CssNumber(_lineHeight),
                ExtraSmallCoef = CssNumber(_coefXS),
                SmallCoef = CssNumber(_coefS),
                MediumCoef = CssNumber(_coefM),
                LargeCoef = CssNumber(_coefL),
                ExtraLargeCoef = CssNumber(_coefXL),
                ExtraExtraLargeCoef = CssNumber(_coefXXL),
                ThinWeight = CssNumber(_weightThin),
                DefaultWeight = CssNumber(_weightDefault),
                BoldWeight = CssNumber(_weightBold),
                ExtraBoldWeight = CssNumber(_weightExtraBold)
            },
            Branches =
            [
                new FbThemeBranch(Branch)
                {
                    SurfaceMix = new FbThemeLayoutSurfaceMix
                    {
                        SurfaceMix = Css(_surfaceMix),
                        AccentOffset = Css(_accentOffset),
                        HighlightOffset = Css(_highlightOffset),
                        HoverOffset = Css(_hoverOffset)
                    },
                    BordersMix = new FbThemeLayoutBordersMix
                    {
                        LightMix = Css(_frameLight),
                        MediumMix = Css(_frameMedium),
                        StrongMix = Css(_frameStrong)
                    },
                    TextVariantMix = new FbThemeFormTextVariantMix
                    {
                        SubtleHighMix = Css(_textSubtle),
                        MutedHighMix = Css(_textMuted),
                        DefaultHighMix = Css(_textDefault),
                        HighlightHighMix = Css(_textHighlight)
                    },
                    Variants = _original.VariantOrder.Select(name =>
                    {
                        var a = _anchors[name];
                        return new FbThemeColorVariant(name)
                        {
                            LayoutColors = new FbThemeLayoutColors { LowAnchor = a.BgLow, HighAnchor = a.BgHigh },
                            FormColors = new FbThemeFormColors { LowAnchor = a.FgLow, HighAnchor = a.FgHigh },
                            FrameColors = a.HasFrameColors ? new FbThemeFormColors { LowAnchor = a.FrameLow, HighAnchor = a.FrameHigh } : null
                        };
                    }).ToList()
                }
            ]
        };

        private static (string Name, string Value, string? Original) MixValue(string name, double value, string? original)
            => (name, Css(value), Css(Percent(original)));

        private static (string Name, string Value, string? Original) Value(string name, string value, string? original)
            => (name, value, original);

        /// <summary>
        /// Appends <paramref name="property"/> with the values that differ from the default theme.
        /// </summary>
        private static void AppendGroup(StringBuilder code, string indent, string property, string type,
            params (string Name, string Value, string? Original)[] values)
        {
            var changed = values.Where(v => v.Value != v.Original?.Trim()).ToList();
            if (changed.Count == 0)
                return;

            var assignments = string.Join(", ", changed.Select(v => $"{v.Name} = \"{Escape(v.Value)}\""));
            code.AppendLine($"{indent}{property} = new {type} {{ {assignments} }},");
        }

        private static string Escape(string value) => value.Replace("\\", "\\\\").Replace("\"", "\\\"");

        private void Set(ref double field, double value)
        {
            if (field == value)
                return;
            field = value;
            Version++;
        }

        private void SetCss(ref string field, string value)
        {
            value = value.Trim();
            if (field == value || value.Length == 0 || value.IndexOfAny([';', '{', '}', '\r', '\n']) >= 0)
                return;
            field = value;
            Version++;
        }

        internal void Touch() => Version++;

        private static double Percent(string? value) => FbColorMath.TryParsePercent(value, out var percent) ? percent : 0d;

        private static double Number(string? value)
            => double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var number) ? number : 0d;

        /// <summary>
        /// A length of the theme in rem. A value in px counts 16px per rem; a unitless one is in rem.
        /// </summary>
        private static double Rem(string? value) => Length(value, pxPerUnit: 16);

        /// <summary>
        /// A length of the theme in px. A value in rem counts 16px per rem; a unitless one is in px.
        /// </summary>
        private static double Px(string? value) => Length(value, pxPerUnit: 1);

        private static double Length(string? value, double pxPerUnit)
        {
            var text = value?.Trim() ?? "";
            var (number, scale) = text.EndsWith("rem", StringComparison.Ordinal) ? (text[..^3], 16 / pxPerUnit)
                : text.EndsWith("px", StringComparison.Ordinal) ? (text[..^2], 1 / pxPerUnit)
                : (text, 1d);
            return Number(number) * scale;
        }

        public static string Css(double percent) => percent.ToString("0.##", CultureInfo.InvariantCulture) + "%";

        public static string CssRem(double rem) => rem.ToString("0.####", CultureInfo.InvariantCulture) + "rem";

        public static string CssPx(double px) => px.ToString("0.##", CultureInfo.InvariantCulture) + "px";

        public static string CssNumber(double value) => value.ToString("0.###", CultureInfo.InvariantCulture);

        /// <summary>
        /// The anchors of one variant: background, text and, when the variant declares them,
        /// frame lines. Colors are kept in the lowercase form of the color picker.
        /// </summary>
        public sealed class Anchors
        {
            private readonly ThemeEditorModel _model;
            private readonly string _bgLow, _bgHigh, _fgLow, _fgHigh;
            private readonly string? _frameLow, _frameHigh;
            private string _currentBgLow, _currentBgHigh, _currentFgLow, _currentFgHigh;
            private string? _currentFrameLow, _currentFrameHigh;

            internal Anchors(ThemeEditorModel model, string bgLow, string bgHigh, string fgLow, string fgHigh, string? frameLow, string? frameHigh)
            {
                _model = model;
                _currentBgLow = _bgLow = bgLow.ToLowerInvariant();
                _currentBgHigh = _bgHigh = bgHigh.ToLowerInvariant();
                _currentFgLow = _fgLow = fgLow.ToLowerInvariant();
                _currentFgHigh = _fgHigh = fgHigh.ToLowerInvariant();
                _currentFrameLow = _frameLow = frameLow?.ToLowerInvariant();
                _currentFrameHigh = _frameHigh = frameHigh?.ToLowerInvariant();
            }

            public string BgLow { get => _currentBgLow; set => Set(ref _currentBgLow, value); }
            public string BgHigh { get => _currentBgHigh; set => Set(ref _currentBgHigh, value); }
            public string FgLow { get => _currentFgLow; set => Set(ref _currentFgLow, value); }
            public string FgHigh { get => _currentFgHigh; set => Set(ref _currentFgHigh, value); }
            public string FrameLow { get => _currentFrameLow ?? _currentBgLow; set => SetFrame(ref _currentFrameLow, value); }
            public string FrameHigh { get => _currentFrameHigh ?? _currentBgHigh; set => SetFrame(ref _currentFrameHigh, value); }

            /// <summary>
            /// Whether the frame lines have their own anchors. Turned on, they start from the
            /// background anchors; turned off, frames go back to the background ones.
            /// </summary>
            public bool HasFrameColors
            {
                get => _currentFrameLow is not null || _currentFrameHigh is not null;
                set
                {
                    if (value == HasFrameColors)
                        return;
                    _currentFrameLow = value ? _currentBgLow : null;
                    _currentFrameHigh = value ? _currentBgHigh : null;
                    _model.Touch();
                }
            }

            public bool Changed => _currentBgLow != _bgLow || _currentBgHigh != _bgHigh || _currentFgLow != _fgLow || _currentFgHigh != _fgHigh
                || _currentFrameLow != _frameLow || _currentFrameHigh != _frameHigh;

            private void Set(ref string field, string value)
            {
                var normalized = value.ToLowerInvariant();
                if (field == normalized)
                    return;
                field = normalized;
                _model.Touch();
            }

            private void SetFrame(ref string? field, string value)
            {
                var normalized = value.ToLowerInvariant();
                if (field == normalized)
                    return;
                _currentFrameLow ??= _currentBgLow;
                _currentFrameHigh ??= _currentBgHigh;
                field = normalized;
                _model.Touch();
            }
        }
    }
}
