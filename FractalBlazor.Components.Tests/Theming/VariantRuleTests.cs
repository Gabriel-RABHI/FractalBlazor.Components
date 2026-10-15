using System.Text.RegularExpressions;
using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Theming;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Forms.Theming.Model;
using FractalBlazor.Components.Layout.Theming;
using FractalBlazor.Components.Layout.Theming.Model;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace FractalBlazor.Components.Tests.Theming;

/// <summary>
/// The rules behind the variant classes: in the Layout stylesheet for the standard variants,
/// in the style of FbTheme for those a theme adds. A variant without a rule is no error.
/// </summary>
public sealed partial class VariantRuleTests : BunitContext
{
    public VariantRuleTests()
    {
        Services.AddFractalBlazorTheming();
    }

    private static RenderFragment Variant<TVariant>(string name) where TVariant : IComponent
        => builder =>
        {
            builder.OpenComponent<TVariant>(0);
            builder.AddAttribute(1, "Variant", name);
            builder.CloseComponent();
        };

    private FbThemeSetup WithBrandVariant() => new("Brand", Services.GetRequiredService<IFbFormThemeRegistry>().Default)
    {
        Branches =
        [
            new FbThemeBranch(FbThemeBranches.Dark)
            {
                Variants = [new FbThemeColorVariant("Brand") { LayoutColors = new FbThemeLayoutColors { MidAnchor = "#123456" } }]
            }
        ]
    };

    [Fact]
    public void A_rule_redirects_the_anchors_and_its_inverted_rule_exchanges_them()
        => Assert.Equal(
            ".fb-v-error{--fb-low-anchor:var(--fb-error-low-anchor);--fb-mid-anchor:var(--fb-error-mid-anchor);--fb-high-anchor:var(--fb-error-high-anchor);}" +
            ".fb-v-error.fb-invert{--fb-low-anchor:var(--fb-error-high-anchor);--fb-high-anchor:var(--fb-error-low-anchor);}",
            FbThemeCssNames.VariantRules("error"));

    [Fact]
    public void The_layout_stylesheet_holds_the_rules_of_every_functional_and_color_variant()
    {
        var css = Whitespace().Replace(Comment().Replace(File.ReadAllText(Path.Combine(RepositoryRoot(),
            "FractalBlazor.Components.Layout", "wwwroot", "fb-components-layout.css")), ""), "");

        foreach (var variant in FbThemeCssNames.StylesheetVariants)
            Assert.Contains(FbThemeCssNames.VariantRules(FbThemeCssNames.Normalize(variant)), css);
    }

    [Fact]
    public void A_theme_writes_only_the_rules_of_the_variants_it_adds()
    {
        var style = Render<FbTheme>(p => p.Add(x => x.Setup, WithBrandVariant())).Find("style").TextContent;

        Assert.Contains(FbThemeCssNames.VariantRules("brand"), style);
        Assert.DoesNotContain(".fb-v-error", style);
    }

    [Fact]
    public void A_scoped_theme_writes_the_rules_it_adds_in_its_own_style()
    {
        var cut = Render<FbTheme>(p => p.Add(x => x.Setup, WithBrandVariant()).Add(x => x.Scoped, true));

        Assert.Equal(FbThemeCssNames.VariantRules("brand"), cut.Find("style").TextContent);
        Assert.NotNull(cut.Find("div.fb-theme-scope.fb-theme"));
    }

    [Fact]
    public void A_theme_with_standard_variants_only_writes_no_rule()
    {
        Assert.DoesNotContain(".fb-v-", Render<FbTheme>().Find("style").TextContent);
        Assert.Empty(Render<FbTheme>(p => p.Add(x => x.Scoped, true)).FindAll("style"));
    }

    [Fact]
    public void An_unknown_variant_is_a_class_without_a_rule()
    {
        var cut = Render<FbTheme>(p => p.Add(x => x.ChildContent, Variant<FbVariant>("Nope")));

        Assert.Contains("fb-v-nope", cut.Find("div.fb-theme-scope").ClassList);
        Assert.DoesNotContain(".fb-v-nope", cut.Find("style").TextContent);
    }

    [Fact]
    public void An_unknown_layout_variant_is_a_class_without_a_rule()
    {
        var cut = Render<FbLayoutTheme>(p => p.Add(x => x.ChildContent, Variant<FbLayoutVariant>("Nope")));

        Assert.Contains("fb-v-nope", cut.Find("div.fb-theme-scope").ClassList);
    }

    private static string RepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, "FractalBlazor.Components.slnx")))
            directory = directory.Parent;
        return directory?.FullName ?? throw new DirectoryNotFoundException("Repository root not found.");
    }

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex Comment();

    [GeneratedRegex(@"\s+")]
    private static partial Regex Whitespace();
}
