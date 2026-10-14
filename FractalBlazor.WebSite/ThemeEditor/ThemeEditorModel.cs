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
    /// theme, and the mixes, master tint and anchors of each branch. Every branch keeps its
    /// own values: switching from Dark to Light and back loses nothing, and
    /// <see cref="Setup"/> declares every branch that was opened. Every change bumps <see cref="Version"/>;
    /// <see cref="Setup"/> is rebuilt only then, so the preview theme is resolved again only
    /// when something was really edited.
    /// </summary>
    public sealed class ThemeEditorModel
    {
        private readonly IFbFormThemeRegistry _registry;
        private readonly FbResolvedTheme _theme;
        private readonly Dictionary<string, BranchValues> _branches = new(StringComparer.OrdinalIgnoreCase);
        private FbThemeSetup? _setup;
        private int _setupVersion = -1;

        public ThemeEditorModel(IFbFormThemeRegistry registry)
        {
            _registry = registry;
            _theme = Resolve(FbThemeBranches.Dark);
            SelectBranch(FbThemeBranches.Dark);
            ResetShape();
            ResetTypography();
        }

        public static IReadOnlyList<string> Branches { get; } = [FbThemeBranches.Dark, FbThemeBranches.Light];

        public int Version { get; private set; }

        /// <summary>
        /// The branch being edited and previewed.
        /// </summary>
        public string Branch { get; private set; } = FbThemeBranches.Dark;

        public string SelectedVariant { get; set; } = FbThemeVariants.Default;

        public IReadOnlyList<string> VariantNames => Current.Original.VariantOrder;

        private BranchValues Current => _branches[Branch];

        // -------- State of the editor itself, kept across navigation
        public int EditTab { get; set; }

        public int PreviewTab { get; set; }

        /// <summary>
        /// The color copied from an anchor, ready to be pasted into another one.
        /// </summary>
        public string? CopiedColor { get; set; }

        // -------- Options of the preview tabs
        public PreviewText VariantsText { get; set; } = PreviewText.Default;

        public PreviewFrame ButtonFrame { get; set; } = PreviewFrame.Medium;

        public PreviewText ButtonText { get; set; } = PreviewText.Default;

        public string FormVariant { get; set; } = FbThemeVariants.Default;

        // -------- Mixes of the edited branch, in percent
        public double NeutralMix { get => Current.NeutralMix; set => Set(ref Current.NeutralMix, value); }
        public double SurfaceOffset { get => Current.SurfaceOffset; set => Set(ref Current.SurfaceOffset, value); }
        public double AccentOffset { get => Current.AccentOffset; set => Set(ref Current.AccentOffset, value); }
        public double HighlightOffset { get => Current.HighlightOffset; set => Set(ref Current.HighlightOffset, value); }
        public double HoverOffset { get => Current.HoverOffset; set => Set(ref Current.HoverOffset, value); }
        public double FrameLight { get => Current.FrameLight; set => Set(ref Current.FrameLight, value); }
        public double FrameMedium { get => Current.FrameMedium; set => Set(ref Current.FrameMedium, value); }
        public double FrameStrong { get => Current.FrameStrong; set => Set(ref Current.FrameStrong, value); }
        public double TextSubtle { get => Current.TextSubtle; set => Set(ref Current.TextSubtle, value); }
        public double TextMuted { get => Current.TextMuted; set => Set(ref Current.TextMuted, value); }
        public double TextDefault { get => Current.TextDefault; set => Set(ref Current.TextDefault, value); }
        public double TextHighlight { get => Current.TextHighlight; set => Set(ref Current.TextHighlight, value); }

        // -------- Master tint of the edited branch: the color every anchor leans toward, and how far
        public string TintColor { get => Current.TintColor; set => SetColor(ref Current.TintColor, value); }
        public double TintMix { get => Current.TintMix; set => Set(ref Current.TintMix, value); }

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

        public Anchors Selected => Current.Anchors[SelectedVariant];

        /// <summary>
        /// Whether <paramref name="branch"/> differs from the default theme.
        /// </summary>
        public bool IsChanged(string branch) => _branches.TryGetValue(branch, out var values) && BranchCode(values).Length > 0;

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
        /// Edits <paramref name="branch"/>. A branch opened for the first time starts from the
        /// default theme; one already edited comes back with its values.
        /// </summary>
        public void SelectBranch(string branch)
        {
            if (!_branches.ContainsKey(branch))
                _branches[branch] = new BranchValues(this, Resolve(branch));

            Branch = branch;
            Version++;
        }

        /// <summary>
        /// Starts the edited branch again from the default theme. The other branch and the
        /// values of the theme are kept.
        /// </summary>
        public void ResetBranch()
        {
            _branches[Branch] = new BranchValues(this, Resolve(Branch));
            Version++;
        }

        /// <summary>
        /// Starts again from the spacings and corners of the default theme.
        /// </summary>
        public void ResetShape()
        {
            _spaceS = Rem(_theme.Spacings.S);
            _spaceM = Rem(_theme.Spacings.M);
            _spaceL = Rem(_theme.Spacings.L);
            _spaceX = Rem(_theme.Spacings.X);
            _radiusS = Rem(_theme.Corners.S);
            _radiusM = Rem(_theme.Corners.M);
            _radiusL = Rem(_theme.Corners.L);
            _radiusX = Rem(_theme.Corners.X);
            Version++;
        }

        /// <summary>
        /// Starts again from the typography of the default theme.
        /// </summary>
        public void ResetTypography()
        {
            var typography = _theme.Typography;
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
        /// Starts everything again from the default theme: the values of the theme and every branch.
        /// </summary>
        public void ResetAll()
        {
            _branches.Clear();
            SelectBranch(Branch);
            ResetShape();
            ResetTypography();
        }

        /// <summary>
        /// The whole state of the editor, to save it.
        /// </summary>
        internal ThemeEditorSnapshot Capture() => new(
            ThemeEditorSnapshot.CurrentFormat,
            Branch, SelectedVariant, EditTab,
            _spaceS, _spaceM, _spaceL, _spaceX,
            _radiusS, _radiusM, _radiusL, _radiusX,
            _textFont, _codeFont, _fontSize, _lineHeight,
            _coefXS, _coefS, _coefM, _coefL, _coefXL, _coefXXL,
            _weightThin, _weightDefault, _weightBold, _weightExtraBold,
            _branches.ToDictionary(pair => pair.Key, pair => pair.Value.Capture()));

        /// <summary>
        /// Comes back to a saved state. Unknown branches and variants are skipped, and invalid
        /// values keep the default ones: an old or damaged save never breaks the editor. A save
        /// made before the three-anchor gradient gives back the values of the theme and the
        /// master tints; its colors meant something else.
        /// </summary>
        internal void Restore(ThemeEditorSnapshot saved)
        {
            _spaceS = saved.SpaceS; _spaceM = saved.SpaceM; _spaceL = saved.SpaceL; _spaceX = saved.SpaceX;
            _radiusS = saved.RadiusS; _radiusM = saved.RadiusM; _radiusL = saved.RadiusL; _radiusX = saved.RadiusX;
            SetCss(ref _textFont, saved.TextFontFamily ?? "");
            SetCss(ref _codeFont, saved.CodeFontFamily ?? "");
            _fontSize = saved.FontSize; _lineHeight = saved.LineHeight;
            _coefXS = saved.CoefXS; _coefS = saved.CoefS; _coefM = saved.CoefM;
            _coefL = saved.CoefL; _coefXL = saved.CoefXL; _coefXXL = saved.CoefXXL;
            _weightThin = saved.WeightThin; _weightDefault = saved.WeightDefault;
            _weightBold = saved.WeightBold; _weightExtraBold = saved.WeightExtraBold;

            foreach (var (name, branch) in saved.Branches ?? [])
            {
                var known = Branches.FirstOrDefault(value => value.Equals(name, StringComparison.OrdinalIgnoreCase));
                if (known is null)
                    continue;

                var values = new BranchValues(this, Resolve(known));
                values.Restore(branch, colors: saved.Format == ThemeEditorSnapshot.CurrentFormat);
                _branches[known] = values;
            }

            EditTab = saved.EditTab;
            SelectBranch(Branches.FirstOrDefault(value => value.Equals(saved.Branch, StringComparison.OrdinalIgnoreCase)) ?? Branch);
            if (Current.Anchors.ContainsKey(saved.SelectedVariant ?? ""))
                SelectedVariant = saved.SelectedVariant!;
        }

        private FbResolvedTheme Resolve(string branch) => _registry.Resolve(_registry.Default.Name, branch);

        /// <summary>
        /// C# of a theme that declares only what differs from the default theme, for the theme
        /// and for every edited branch.
        /// </summary>
        public string ToCSharp()
        {
            const string ThemeIndent = "    ";
            var typography = _theme.Typography;

            var theme = new StringBuilder();
            AppendGroup(theme, ThemeIndent, "Spacings", "FbThemeLayoutSpacings",
                Value("S", CssRem(_spaceS), CssRem(Rem(_theme.Spacings.S))),
                Value("M", CssRem(_spaceM), CssRem(Rem(_theme.Spacings.M))),
                Value("L", CssRem(_spaceL), CssRem(Rem(_theme.Spacings.L))),
                Value("X", CssRem(_spaceX), CssRem(Rem(_theme.Spacings.X))));
            AppendGroup(theme, ThemeIndent, "Corners", "FbThemeLayoutCorners",
                Value("S", CssRem(_radiusS), CssRem(Rem(_theme.Corners.S))),
                Value("M", CssRem(_radiusM), CssRem(Rem(_theme.Corners.M))),
                Value("L", CssRem(_radiusL), CssRem(Rem(_theme.Corners.L))),
                Value("X", CssRem(_radiusX), CssRem(Rem(_theme.Corners.X))));
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

            var branches = Branches
                .Where(_branches.ContainsKey)
                .Select(name => (Name: name, Code: BranchCode(_branches[name])))
                .Where(branch => branch.Code.Length > 0)
                .ToList();

            if (theme.Length == 0 && branches.Count == 0)
                return "// Aucune valeur modifiée : le thème par défaut s'applique tel quel.";

            var body = new StringBuilder(theme.ToString());
            if (branches.Count > 0)
            {
                body.AppendLine("    Branches =");
                body.AppendLine("    [");
                for (var i = 0; i < branches.Count; i++)
                {
                    body.AppendLine($"        new FbThemeBranch(FbThemeBranches.{branches[i].Name})");
                    body.AppendLine("        {");
                    body.Append(branches[i].Code.TrimEnd().TrimEnd(',')).AppendLine();
                    body.AppendLine(i < branches.Count - 1 ? "        }," : "        }");
                }
                body.AppendLine("    ]");
            }

            var code = new StringBuilder();
            code.AppendLine("var myTheme = new FbThemeSetup(\"MyTheme\", registry.Default)");
            code.AppendLine("{");
            code.Append(body.ToString().TrimEnd().TrimEnd(',')).AppendLine();
            code.Append("};");
            return code.ToString();
        }

        /// <summary>
        /// The assignments of one branch that differ from the default theme, or nothing.
        /// </summary>
        private static string BranchCode(BranchValues values)
        {
            const string BranchIndent = "            ";
            var original = values.Original;

            var branch = new StringBuilder();
            AppendGroup(branch, BranchIndent, "MasterTint", "FbThemeMasterTint",
                Value("ColorTint", values.TintColor, original.MasterTint.ColorTint?.ToLowerInvariant()),
                MixValue("TintMix", values.TintMix, original.MasterTint.TintMix));
            AppendGroup(branch, BranchIndent, "SurfaceMix", "FbThemeLayoutSurfaceMix",
                MixValue("NeutralMix", values.NeutralMix, original.SurfaceMix.NeutralMix),
                MixValue("SurfaceOffset", values.SurfaceOffset, original.SurfaceMix.SurfaceOffset),
                MixValue("AccentOffset", values.AccentOffset, original.SurfaceMix.AccentOffset),
                MixValue("HighlightOffset", values.HighlightOffset, original.SurfaceMix.HighlightOffset),
                MixValue("HoverOffset", values.HoverOffset, original.SurfaceMix.HoverOffset));
            AppendGroup(branch, BranchIndent, "BordersMix", "FbThemeLayoutBordersMix",
                MixValue("LightMix", values.FrameLight, original.BordersMix.LightMix),
                MixValue("MediumMix", values.FrameMedium, original.BordersMix.MediumMix),
                MixValue("StrongMix", values.FrameStrong, original.BordersMix.StrongMix));
            AppendGroup(branch, BranchIndent, "TextVariantMix", "FbThemeFormTextVariantMix",
                MixValue("SubtleHighMix", values.TextSubtle, original.TextVariant.SubtleHighMix),
                MixValue("MutedHighMix", values.TextMuted, original.TextVariant.MutedHighMix),
                MixValue("DefaultHighMix", values.TextDefault, original.TextVariant.DefaultHighMix),
                MixValue("HighlightHighMix", values.TextHighlight, original.TextVariant.HighlightHighMix));

            var variants = original.VariantOrder.Where(name => values.Anchors[name].Changed).ToList();
            if (variants.Count > 0)
            {
                branch.AppendLine($"{BranchIndent}Variants =");
                branch.AppendLine($"{BranchIndent}[");
                for (var i = 0; i < variants.Count; i++)
                {
                    var a = values.Anchors[variants[i]];
                    branch.AppendLine($"{BranchIndent}    new FbThemeColorVariant(\"{variants[i]}\")");
                    branch.AppendLine($"{BranchIndent}    {{");
                    branch.AppendLine($"{BranchIndent}        LayoutColors = new FbThemeLayoutColors {{ LowAnchor = \"{a.Low}\", MidAnchor = \"{a.Mid}\", HighAnchor = \"{a.High}\" }}");
                    branch.AppendLine(i < variants.Count - 1 ? $"{BranchIndent}    }}," : $"{BranchIndent}    }}");
                }
                branch.AppendLine($"{BranchIndent}]");
            }

            return branch.ToString();
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
            Branches = _branches.Select(pair => BuildBranch(pair.Key, pair.Value)).ToList()
        };

        private static FbThemeBranch BuildBranch(string name, BranchValues values) => new(name)
        {
            MasterTint = new FbThemeMasterTint
            {
                ColorTint = values.TintColor,
                TintMix = Css(values.TintMix)
            },
            SurfaceMix = new FbThemeLayoutSurfaceMix
            {
                NeutralMix = Css(values.NeutralMix),
                SurfaceOffset = Css(values.SurfaceOffset),
                AccentOffset = Css(values.AccentOffset),
                HighlightOffset = Css(values.HighlightOffset),
                HoverOffset = Css(values.HoverOffset)
            },
            BordersMix = new FbThemeLayoutBordersMix
            {
                LightMix = Css(values.FrameLight),
                MediumMix = Css(values.FrameMedium),
                StrongMix = Css(values.FrameStrong)
            },
            TextVariantMix = new FbThemeFormTextVariantMix
            {
                SubtleHighMix = Css(values.TextSubtle),
                MutedHighMix = Css(values.TextMuted),
                DefaultHighMix = Css(values.TextDefault),
                HighlightHighMix = Css(values.TextHighlight)
            },
            Variants = values.Original.VariantOrder.Select(variant =>
            {
                var a = values.Anchors[variant];
                return new FbThemeColorVariant(variant)
                {
                    LayoutColors = new FbThemeLayoutColors { LowAnchor = a.Low, MidAnchor = a.Mid, HighAnchor = a.High }
                };
            }).ToList()
        };

        /// <summary>
        /// The values of one branch: its mixes, in percent, and the anchors of its variants,
        /// starting from the default theme.
        /// </summary>
        private sealed class BranchValues
        {
            public readonly FbResolvedTheme Original;
            public readonly Dictionary<string, Anchors> Anchors = new(StringComparer.OrdinalIgnoreCase);
            public double NeutralMix, SurfaceOffset, AccentOffset, HighlightOffset, HoverOffset, FrameLight, FrameMedium, FrameStrong,
                TextSubtle, TextMuted, TextDefault, TextHighlight, TintMix;
            public string TintColor;

            public BranchValues(ThemeEditorModel model, FbResolvedTheme original)
            {
                Original = original;
                TintColor = original.MasterTint.ColorTint!.ToLowerInvariant();
                TintMix = Percent(original.MasterTint.TintMix);
                NeutralMix = Percent(original.SurfaceMix.NeutralMix);
                SurfaceOffset = Percent(original.SurfaceMix.SurfaceOffset);
                AccentOffset = Percent(original.SurfaceMix.AccentOffset);
                HighlightOffset = Percent(original.SurfaceMix.HighlightOffset);
                HoverOffset = Percent(original.SurfaceMix.HoverOffset);
                FrameLight = Percent(original.BordersMix.LightMix);
                FrameMedium = Percent(original.BordersMix.MediumMix);
                FrameStrong = Percent(original.BordersMix.StrongMix);
                TextSubtle = Percent(original.TextVariant.SubtleHighMix);
                TextMuted = Percent(original.TextVariant.MutedHighMix);
                TextDefault = Percent(original.TextVariant.DefaultHighMix);
                TextHighlight = Percent(original.TextVariant.HighlightHighMix);

                foreach (var name in original.VariantOrder)
                {
                    var variant = original.GetVariant(name);
                    Anchors[name] = new Anchors(model,
                        variant.LayoutColors.LowAnchor!, variant.LayoutColors.MidAnchor!, variant.LayoutColors.HighAnchor!);
                }
            }

            public BranchSnapshot Capture() => new(
                NeutralMix, SurfaceOffset, AccentOffset, HighlightOffset, HoverOffset,
                FrameLight, FrameMedium, FrameStrong,
                TextSubtle, TextMuted, TextDefault, TextHighlight,
                Anchors.ToDictionary(pair => pair.Key, pair => pair.Value.Capture()),
                TintColor, TintMix);

            /// <summary>
            /// Takes back a saved branch. A save made before the three-anchor gradient
            /// (<paramref name="colors"/> false) only gives back its master tint: its mixes and
            /// anchors meant something else, and the branch keeps those of the default theme.
            /// </summary>
            public void Restore(BranchSnapshot saved, bool colors)
            {
                TintColor = HexColor(saved.TintColor) ?? TintColor;
                TintMix = saved.TintMix ?? TintMix;
                if (!colors)
                    return;

                NeutralMix = saved.NeutralMix; SurfaceOffset = saved.SurfaceOffset; AccentOffset = saved.AccentOffset;
                HighlightOffset = saved.HighlightOffset; HoverOffset = saved.HoverOffset;
                FrameLight = saved.FrameLight; FrameMedium = saved.FrameMedium; FrameStrong = saved.FrameStrong;
                TextSubtle = saved.TextSubtle; TextMuted = saved.TextMuted;
                TextDefault = saved.TextDefault; TextHighlight = saved.TextHighlight;

                foreach (var (name, anchors) in saved.Anchors ?? [])
                {
                    if (Anchors.TryGetValue(name, out var current))
                        current.Restore(anchors);
                }
            }
        }

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

        /// <summary>
        /// Sets a color from the picker; a value that is not a hexadecimal color is ignored.
        /// </summary>
        private void SetColor(ref string field, string value)
        {
            var color = HexColor(value);
            if (color is null || field == color)
                return;
            field = color;
            Version++;
        }

        /// <summary>
        /// The lowercase form of a <c>#RGB</c> or <c>#RRGGBB</c> color, or <c>null</c>.
        /// </summary>
        private static string? HexColor(string? value)
            => value is { Length: 4 or 7 } && value[0] == '#' && value.Skip(1).All(char.IsAsciiHexDigit) ? value.ToLowerInvariant() : null;

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
        /// The gradient of one variant: its anchors at 0 %, 50 % and 100 %. Colors are kept in
        /// the lowercase form of the color picker.
        /// </summary>
        public sealed class Anchors
        {
            private readonly ThemeEditorModel _model;
            private readonly string _low, _mid, _high;
            private string _currentLow, _currentMid, _currentHigh;

            internal Anchors(ThemeEditorModel model, string low, string mid, string high)
            {
                _model = model;
                _currentLow = _low = low.ToLowerInvariant();
                _currentMid = _mid = mid.ToLowerInvariant();
                _currentHigh = _high = high.ToLowerInvariant();
            }

            public string Low { get => _currentLow; set => Set(ref _currentLow, value); }
            public string Mid { get => _currentMid; set => Set(ref _currentMid, value); }
            public string High { get => _currentHigh; set => Set(ref _currentHigh, value); }

            internal AnchorSnapshot Capture() => new(_currentLow, _currentMid, _currentHigh);

            /// <summary>
            /// Takes back saved colors; a value that is not a color is left as it is.
            /// </summary>
            internal void Restore(AnchorSnapshot saved)
            {
                _currentLow = HexColor(saved.Low) ?? _currentLow;
                _currentMid = HexColor(saved.Mid) ?? _currentMid;
                _currentHigh = HexColor(saved.High) ?? _currentHigh;
            }

            public bool Changed => _currentLow != _low || _currentMid != _mid || _currentHigh != _high;

            private void Set(ref string field, string value)
            {
                var normalized = value.ToLowerInvariant();
                if (field == normalized)
                    return;
                field = normalized;
                _model.Touch();
            }
        }
    }
}
