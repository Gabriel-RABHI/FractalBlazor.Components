using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Components.Inputs;
using FractalBlazor.Components.Forms.Core;
using FractalBlazor.Components.Forms.Theming.Constants;
using Microsoft.AspNetCore.Components;

namespace FractalBlazor.Components.Tests.Forms;

public sealed record RenameAction(string Name) : IStateAction<string>;

public sealed record ResetAction : IStateAction;

public sealed class PersonState
{
    public string Name { get; private set; } = "Ada";

    public int Resets { get; private set; }

    public void Handle(RenameAction action) => Name = action.Name;

    public void Handle(ResetAction action) => Resets++;
}

public sealed class InputTextTests : BunitContext
{
    [Fact]
    public void Change_sends_the_action_to_the_state()
    {
        var state = new PersonState();
        var cut = Render<FbInputText<RenameAction>>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Value, state.Name));

        cut.Find("input").Change("Grace");

        Assert.Equal("Grace", state.Name);
    }

    [Fact]
    public void Value_changed_supports_binding_without_a_state()
    {
        string? bound = null;
        var cut = Render<FbInputText<FbSetValue<string>>>(p => p
            .Add(x => x.Value, "Ada")
            .Add(x => x.ValueChanged, EventCallback.Factory.Create<string>(this, value => bound = value)));

        cut.Find("input").Change("Grace");

        Assert.Equal("Grace", bound);
    }

    [Fact]
    public void Immediate_sends_on_every_keystroke()
    {
        var state = new PersonState();
        var cut = Render<FbInputText<RenameAction>>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Immediate, true));

        cut.Find("input").Input("Gr");

        Assert.Equal("Gr", state.Name);
    }

    [Fact]
    public void Disabled_field_sends_nothing_and_uses_the_disabled_variant()
    {
        var state = new PersonState();
        var cut = Render<FbInputText<RenameAction>>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Disabled, true));

        Assert.True(cut.Find("input").HasAttribute("disabled"));
        var root = cut.Find(".fb-field");
        Assert.Contains("fb-variant", root.ClassList);
        Assert.Contains("var(--fb-disabled-bg-low-anchor)", root.GetAttribute("style"));

        cut.Find("input").Change("Grace");
        Assert.Equal("Ada", state.Name);
    }

    [Fact]
    public void Field_without_variant_inherits_the_parent_one()
    {
        var cut = Render<FbInputText<RenameAction>>();

        var root = cut.Find(".fb-field");
        Assert.DoesNotContain("fb-variant", root.ClassList);
        Assert.False(root.HasAttribute("style"));
        Assert.Empty(cut.FindAll(".fb-theme-scope"));
    }

    [Fact]
    public void Switching_to_error_only_changes_the_root_attributes()
    {
        var cut = Render<FbInputText<RenameAction>>();
        var structure = cut.FindAll("*").Select(element => element.TagName).ToList();

        cut.Render(p => p.Add(x => x.Variant, FbThemeVariants.Error));

        Assert.Equal(structure, cut.FindAll("*").Select(element => element.TagName));
        var root = cut.Find(".fb-field");
        Assert.Contains("fb-variant", root.ClassList);
        Assert.Contains("var(--fb-error-bg-low-anchor)", root.GetAttribute("style"));
    }

    [Fact]
    public void Escape_restores_the_value_from_before_the_focus()
    {
        var state = new PersonState();
        var cut = Render<FbInputText<RenameAction>>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Value, "Ada")
            .Add(x => x.Immediate, true));

        var input = cut.Find("input");
        input.Focus();
        input.Input("Grace");
        cut.Find("input").KeyUp(key: "Escape");

        Assert.Equal("Ada", state.Name);
        Assert.Equal("Ada", cut.Find("input").GetAttribute("value"));
    }

    [Fact]
    public void Untyped_action_reaches_the_matching_handle_method()
    {
        var state = new PersonState();

        FbActionDispatcher.Dispatch(state, new ResetAction());
        FbActionDispatcher.Dispatch(state, new RenameAction("Grace"));

        Assert.Equal(1, state.Resets);
        Assert.Equal("Grace", state.Name);
    }
}
