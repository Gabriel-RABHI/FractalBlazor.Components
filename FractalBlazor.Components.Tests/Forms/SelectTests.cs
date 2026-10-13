using FractalBlazor.Components.Common.Attributes;
using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms;
using FractalBlazor.Components.Forms.Components.Selectors;
using FractalBlazor.Components.Forms.Mapping;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace FractalBlazor.Components.Tests.Forms;

public sealed record Country(string Code, string Name);

public sealed record SetCountryAction(Country? Country) : IStateAction<Country?>;

public sealed class AddressState
{
    public static readonly IReadOnlyList<Country?> Countries =
        [new("FR", "France"), new("JP", "Japon"), new("CA", "Canada")];

    public Country? Country { get; private set; }

    public void Handle(SetCountryAction action) => Country = action.Country;
}

[ViewFor<FbViewSelector>(typeof(Country), FbViewSelector.Item)]
public sealed class CountryItemView : FbComponentBase
{
    [Parameter]
    public object? Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        var country = (Country)Item!;
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", "country");
        builder.AddContent(2, $"{country.Name} ({country.Code})");
        builder.CloseElement();
    }
}

[ViewFor<TestSelector>(typeof(Country), TestSelector.Summary)]
public sealed class CountryCodeView : FbComponentBase
{
    [Parameter]
    public object? Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "span");
        builder.AddAttribute(1, "class", "code");
        builder.AddContent(2, ((Country)Item!).Code);
        builder.CloseElement();
    }
}

public sealed class SelectTests : BunitContext
{
    private static readonly IReadOnlyList<object> Mixed = ["Paris", Priority.VeryHigh, new Country("JP", "Japon")];

    public SelectTests()
    {
        Services.AddFractalBlazor();
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private IRenderedComponent<FbSelect<Country?, SetCountryAction>> RenderCountries(AddressState state)
        => Render<FbSelect<Country?, SetCountryAction>>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Value, state.Country)
            .Add(x => x.Items, AddressState.Countries)
            .Add(x => x.Placeholder, "Pays"));

    [Fact]
    public void Closed_select_shows_the_placeholder_and_no_option()
    {
        var cut = RenderCountries(new AddressState());

        var anchor = cut.Find("[role=combobox]");
        Assert.Equal("false", anchor.GetAttribute("aria-expanded"));
        Assert.Contains("Pays", anchor.TextContent);
        Assert.Empty(cut.FindAll("[role=option]"));
    }

    [Fact]
    public void A_polymorphic_list_renders_each_element_with_its_own_view()
    {
        var cut = Render<FbSelect<object, FbSetValue<object>>>(p => p.Add(x => x.Items, Mixed));

        cut.Find(".fb-dropdown-anchor").Click();

        var options = cut.FindAll("[role=option]");
        Assert.Equal(3, options.Count);
        Assert.Equal("Paris", options[0].TextContent);
        Assert.Equal("Très haute", options[1].TextContent);
        Assert.NotNull(options[2].QuerySelector("span.country"));
        Assert.Equal("Japon (JP)", options[2].TextContent);
    }

    [Fact]
    public void Choosing_an_option_sends_the_item_and_closes()
    {
        var state = new AddressState();
        var cut = RenderCountries(state);

        cut.Find(".fb-dropdown-anchor").Click();
        cut.FindAll("[role=option]")[1].Click();

        Assert.Same(AddressState.Countries[1], state.Country);
        Assert.Empty(cut.FindAll("[role=option]"));
        Assert.Equal("Japon (JP)", cut.Find(".fb-select-anchor .fb-select-value").TextContent);
    }

    [Fact]
    public void Current_value_is_marked_selected()
    {
        var state = new AddressState();
        state.Handle(new SetCountryAction(AddressState.Countries[2]));
        var cut = RenderCountries(state);

        cut.Find(".fb-dropdown-anchor").Click();

        var selected = cut.FindAll("[role=option]").Single(o => o.GetAttribute("aria-selected") == "true");
        Assert.Equal("Canada (CA)", selected.TextContent);
    }

    [Fact]
    public void Keyboard_moves_through_the_options_and_chooses()
    {
        var state = new AddressState();
        var cut = RenderCountries(state);
        var anchor = () => cut.Find("[role=combobox]");

        anchor().KeyDown(key: "ArrowDown");
        Assert.Equal("true", anchor().GetAttribute("aria-expanded"));
        Assert.EndsWith("-0", anchor().GetAttribute("aria-activedescendant"));

        anchor().KeyDown(key: "ArrowDown");
        anchor().KeyDown(key: "End");
        anchor().KeyDown(key: "ArrowUp");
        Assert.EndsWith("-1", anchor().GetAttribute("aria-activedescendant"));

        anchor().KeyDown(key: "Enter");

        Assert.Same(AddressState.Countries[1], state.Country);
        Assert.Equal("false", anchor().GetAttribute("aria-expanded"));
    }

    [Fact]
    public void Escape_closes_without_choosing()
    {
        var state = new AddressState();
        var cut = RenderCountries(state);

        cut.Find("[role=combobox]").KeyDown(key: "ArrowDown");
        cut.Find(".fb-dropdown-container").KeyDown(key: "Escape");

        Assert.Null(state.Country);
        Assert.Empty(cut.FindAll("[role=option]"));
    }

    [Fact]
    public void Item_selector_picks_other_views()
    {
        var cut = Render<FbSelect<Country?, SetCountryAction>>(p => p
            .Add(x => x.Items, AddressState.Countries)
            .Add(x => x.Value, AddressState.Countries[0])
            .Add(x => x.ItemSelector, TestSelector.Summary));

        Assert.Equal("FR", cut.Find(".fb-select-anchor span.code").TextContent);
    }

    [Fact]
    public void Disabled_select_does_not_open()
    {
        var cut = Render<FbSelect<Country?, SetCountryAction>>(p => p
            .Add(x => x.Items, AddressState.Countries)
            .Add(x => x.Disabled, true));

        Assert.Equal("-1", cut.Find("[role=combobox]").GetAttribute("tabindex"));
        Assert.DoesNotContain("onclick", cut.Find(".fb-dropdown-anchor").OuterHtml);
        cut.Find("[role=combobox]").KeyDown(key: "ArrowDown");
        Assert.Empty(cut.FindAll("[role=option]"));
    }
}
