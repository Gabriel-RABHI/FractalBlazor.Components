using System.Linq.Expressions;
using FractalBlazor.Components.Forms.Components.Actions;
using FractalBlazor.Components.Forms.Theming.Constants;
using FractalBlazor.Components.Layout.Utilities;

namespace FractalBlazor.Components.Tests.Forms;

public sealed class ButtonTests : BunitContext
{
    [Fact]
    public void Default_button_is_a_medium_wireframe()
    {
        var cut = Render<FbButton>(p => p.AddChildContent("Save"));

        var button = cut.Find("button");
        Assert.Equal("button", button.GetAttribute("type"));
        Assert.DoesNotContain(button.ClassList, name => name.StartsWith("fb-wb", StringComparison.Ordinal));
        Assert.Equal(ClassesOf(p => p.Add(x => x.WBN, true).Add(x => x.WFS, true).Add(x => x.PHL, true).Add(x => x.PVM, true)),
            button.GetAttribute("class"));
        Assert.Contains("fb-wfs", button.ClassList);
        Assert.Contains("fb-hover", button.ClassList);
        Assert.Contains(FbAtoms.NameOf("border-radius:var(--fb-radius-m)"), button.ClassList);
        Assert.False(button.HasAttribute("style"));
        Assert.Equal("Save", button.TextContent);
    }

    [Fact]
    public void Defaults_can_be_replaced_or_removed()
    {
        var cut = Render<FbButton>(p => p.Add(x => x.WBH, true).Add(x => x.WFN, true));

        var button = cut.Find("button");
        Assert.Contains("fb-wbh", button.ClassList);
        Assert.DoesNotContain("fb-wba", button.ClassList);
        Assert.DoesNotContain("fb-wfm", button.ClassList);
    }

    [Fact]
    public void Click_sends_the_action_then_raises_on_click()
    {
        var state = new PersonState();
        var clicks = 0;
        var cut = Render<FbButton>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Action, new ResetAction())
            .Add(x => x.OnClick, () => clicks++));

        cut.Find("button").Click();

        Assert.Equal(1, state.Resets);
        Assert.Equal(1, clicks);
    }

    [Fact]
    public void Button_without_behaviour_registers_no_click()
    {
        var cut = Render<FbButton>();

        Assert.DoesNotContain("onclick", cut.Markup);
    }

    [Fact]
    public void Disabled_button_is_inert_and_uses_the_disabled_variant()
    {
        var state = new PersonState();
        var cut = Render<FbButton>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Action, new ResetAction())
            .Add(x => x.D, true));

        Assert.True(cut.Find("button").HasAttribute("disabled"));
        Assert.DoesNotContain("onclick", cut.Markup);
        Assert.Contains("fb-variant", cut.Find("button").ClassList);
        Assert.Contains("fb-v-disabled", cut.Find("button").ClassList);
        Assert.Null(cut.Find("button").GetAttribute("style"));
    }

    [Fact]
    public void A_disabled_button_takes_the_disabled_variant_whatever_its_variant()
    {
        var cut = Render<FbButton>(p => p.Add(x => x.D, true).Add(x => x.Variant, FbThemeVariants.Error));

        Assert.Contains("fb-v-disabled", cut.Find("button").ClassList);
        Assert.DoesNotContain("fb-v-error", cut.Find("button").ClassList);

        cut.Render(p => p.Add(x => x.D, false).Add(x => x.Variant, FbThemeVariants.Error));

        Assert.Contains("fb-v-error", cut.Find("button").ClassList);
        Assert.DoesNotContain("fb-v-disabled", cut.Find("button").ClassList);
    }

    [Fact]
    public void Variant_gives_the_functional_state()
    {
        var cut = Render<FbButton>(p => p.Add(x => x.Variant, FbThemeVariants.Error));

        Assert.Contains("fb-variant", cut.Find("button").ClassList);
        Assert.Contains("fb-v-error", cut.Find("button").ClassList);
        Assert.Null(cut.Find("button").GetAttribute("style"));
        Assert.Empty(cut.FindAll(".fb-theme-scope"));
    }

    private string? ClassesOf(Action<ComponentParameterCollectionBuilder<FbButton>> parameters)
        => Render(parameters).Find("button").GetAttribute("class");

    public static TheoryData<string, string, string> Shortcuts => new()
    {
        { nameof(FbButton.S), nameof(FbButton.PHM), nameof(FbButton.PVS) },
        { nameof(FbButton.M), nameof(FbButton.PHL), nameof(FbButton.PVM) },
        { nameof(FbButton.L), nameof(FbButton.PHX), nameof(FbButton.PVL) },
        { nameof(FbButton.KW), nameof(FbButton.WBN), nameof(FbButton.WFS) },
        { nameof(FbButton.KS), nameof(FbButton.WBS), nameof(FbButton.WFS) },
        { nameof(FbButton.KP), nameof(FbButton.WBH), nameof(FbButton.WFS) },
        { nameof(FbButton.KT), nameof(FbButton.WBS), nameof(FbButton.WFN) }
    };

    /// <summary>
    /// A size sets the padding, a kind the fill and the frame: each one gives exactly the
    /// classes of the two Layout flags it stands for.
    /// </summary>
    [Theory]
    [MemberData(nameof(Shortcuts))]
    public void A_size_or_a_kind_stands_for_two_layout_flags(string shortcut, string first, string second)
        => Assert.Equal(
            ClassesOf(p => p.Add(Flag(shortcut), true)),
            ClassesOf(p => p.Add(Flag(first), true).Add(Flag(second), true)));

    private static Expression<Func<FbButton, bool>> Flag(string name)
    {
        var button = Expression.Parameter(typeof(FbButton));
        return Expression.Lambda<Func<FbButton, bool>>(Expression.Property(button, name), button);
    }

    [Fact]
    public void A_layout_flag_written_after_a_kind_corrects_it()
    {
        var button = Render<FbButton>(p => p.Add(x => x.KP, true).Add(x => x.WFN, true)).Find("button");

        Assert.Contains("fb-wbh", button.ClassList);
        Assert.DoesNotContain("fb-wfm", button.ClassList);
    }

    [Fact]
    public void The_content_is_the_child_content_alone()
    {
        var cut = Render<FbButton>(p => p.AddChildContent("<span>Find</span>"));

        Assert.Equal("<span>Find</span>", cut.Find("button").InnerHtml);
    }
}
