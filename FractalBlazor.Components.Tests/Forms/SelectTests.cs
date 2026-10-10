using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Inputs;

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

public sealed class SelectTests : BunitContext
{
    private IRenderedComponent<FbSelect<Country?, SetCountryAction>> RenderSelect(AddressState state, string? placeholder = null)
        => Render<FbSelect<Country?, SetCountryAction>>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Value, state.Country)
            .Add(x => x.Items, AddressState.Countries)
            .Add(x => x.ItemText, country => country!.Name)
            .Add(x => x.Placeholder, placeholder));

    [Fact]
    public void Lists_the_items_with_their_text()
    {
        var cut = RenderSelect(new AddressState(), "Choose a country");

        var options = cut.FindAll("option");
        Assert.Equal(["Choose a country", "France", "Japon", "Canada"], options.Select(o => o.TextContent));
        Assert.True(options[0].HasAttribute("selected"));
    }

    [Fact]
    public void Choosing_an_option_sends_the_item_itself()
    {
        var state = new AddressState();
        var cut = RenderSelect(state);

        cut.Find("select").Change("1");

        Assert.Same(AddressState.Countries[1], state.Country);
    }

    [Fact]
    public void Current_value_is_selected()
    {
        var state = new AddressState();
        state.Handle(new SetCountryAction(AddressState.Countries[2]));

        var cut = RenderSelect(state);

        var selected = cut.FindAll("option").Single(o => o.HasAttribute("selected"));
        Assert.Equal("Canada", selected.TextContent);
    }
}
