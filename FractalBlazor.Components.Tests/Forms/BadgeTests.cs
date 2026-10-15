using FractalBlazor.Components.Forms;
using FractalBlazor.Components.Forms.Components.Display;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Layout.Theming.Model;

namespace FractalBlazor.Components.Tests.Forms;

/// <summary>
/// The badge: an inline, round row that shows a label or a content of its own, or a dot.
/// </summary>
public sealed class BadgeTests : BunitContext
{
    public BadgeTests() => Services.AddFractalBlazor();

    private IRenderedComponent<FbBadge> Badge(Action<ComponentParameterCollectionBuilder<FbBadge>> parameters)
        => Render<FbBadge>(parameters);

    [Fact]
    public void A_badge_is_an_inline_round_row_filled_with_its_full_color()
    {
        var root = Badge(p => p.Add(x => x.Label, "Badge")).Find("span.fb-badge");

        Assert.Equal(["fb-badge", "fb-row"], root.ClassList.Take(2));
        Assert.Contains("fb-wbh", root.ClassList);
        Assert.Contains("fb-wrr", root.ClassList);
        var label = root.QuerySelector(".fb-txt");
        Assert.NotNull(label);
        Assert.Equal("Badge", label.TextContent);
        Assert.Contains("fb-t-s", label.ClassList);
    }

    [Fact]
    public void Its_defaults_are_those_of_a_row_with_PHM_and_PVS()
    {
        var badge = Badge(p => p.Add(x => x.Label, "Badge")).Instance;

        Assert.True(badge.PHM && badge.PVS && badge.WRR && badge.WBH && badge.NoFlex && badge.JVC && badge.GS);
    }

    [Fact]
    public void A_content_takes_the_place_of_the_label()
    {
        var root = Badge(p => p.Add(x => x.Label, "Badge").AddChildContent("<i>3</i> new")).Find("span.fb-badge");

        Assert.Equal("3 new", root.TextContent);
        Assert.Null(root.QuerySelector(".fb-txt"));
    }

    [Fact]
    public void Layout_flags_change_its_look()
    {
        var outline = Badge(p => p.Add(x => x.Label, "Outline").Add(x => x.WBN, true).Add(x => x.WFM, true)).Find("span.fb-badge");
        var soft = Badge(p => p.Add(x => x.Label, "Soft").Add(x => x.WBA, true)).Find("span.fb-badge");

        Assert.DoesNotContain("fb-wbh", outline.ClassList);
        Assert.Contains("fb-wfm", outline.ClassList);
        Assert.Contains("fb-wba", soft.ClassList);
        Assert.DoesNotContain("fb-wbh", soft.ClassList);
    }

    [Fact]
    public void A_variant_colors_the_badge_on_its_root()
    {
        var root = Badge(p => p.Add(x => x.Label, "Destructive").Add(x => x.Variant, FbThemeVariants.Error)).Find("span.fb-badge");

        Assert.EndsWith(" " + FbThemeCssNames.VariantClasses(FbThemeVariants.Error), root.GetAttribute("class"));
    }

    [Fact]
    public void A_dot_has_no_content_and_is_named_by_its_title()
    {
        var root = Badge(p => p.Add(x => x.Label, "ignored").Add(x => x.Dot, true).Add(x => x.Title, "En ligne")).Find("span.fb-badge");

        Assert.Contains("fb-badge-dot", root.ClassList);
        Assert.Equal(("img", "En ligne", "En ligne"), (root.GetAttribute("role"), root.GetAttribute("aria-label"), root.GetAttribute("title")));
        Assert.Equal("", root.InnerHtml);
    }

    [Fact]
    public void A_dot_no_longer_passed_falls_back()
    {
        var cut = Badge(p => p.Add(x => x.Label, "Badge").Add(x => x.Dot, true));

        cut.Render(p => p.Add(x => x.Label, "Badge"));

        var root = cut.Find("span.fb-badge");
        Assert.DoesNotContain("fb-badge-dot", root.ClassList);
        Assert.False(root.HasAttribute("role"));
        Assert.Equal("Badge", root.TextContent);
    }
}
