using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Inputs;

namespace FractalBlazor.Components.Tests.Forms;

public sealed record SetAgeAction(int Age) : IStateAction<int>;

public sealed record SetPriceAction(decimal Price) : IStateAction<decimal>;

public sealed record SetBirthAction(DateOnly? Birth) : IStateAction<DateOnly?>;

public sealed class ProfileState
{
    public int Age { get; private set; } = 30;

    public decimal Price { get; private set; }

    public DateOnly? Birth { get; private set; } = new(1990, 5, 17);

    public void Handle(SetAgeAction action) => Age = action.Age;

    public void Handle(SetPriceAction action) => Price = action.Price;

    public void Handle(SetBirthAction action) => Birth = action.Birth;
}

public sealed class NumberAndDateTests : BunitContext
{
    [Fact]
    public void Number_is_rendered_and_parsed_in_the_invariant_culture()
    {
        var state = new ProfileState();
        var cut = Render<FbInputNumber<decimal, SetPriceAction>>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Value, 12.5m));

        Assert.Equal("number", cut.Find("input").GetAttribute("type"));
        Assert.Equal("12.5", cut.Find("input").GetAttribute("value"));

        cut.Find("input").Change("3.75");

        Assert.Equal(3.75m, state.Price);
    }

    [Fact]
    public void Number_is_clamped_to_its_bounds()
    {
        var state = new ProfileState();
        var cut = Render<FbInputNumber<int, SetAgeAction>>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Min, 0)
            .Add(x => x.Max, 120));

        Assert.Equal("0", cut.Find("input").GetAttribute("min"));
        Assert.Equal("120", cut.Find("input").GetAttribute("max"));

        cut.Find("input").Change("150");

        Assert.Equal(120, state.Age);
    }

    [Fact]
    public void Text_that_is_not_a_number_is_not_sent()
    {
        var state = new ProfileState();
        var cut = Render<FbInputNumber<int, SetAgeAction>>(p => p.Add(x => x.Handler, state));

        cut.Find("input").Change("");

        Assert.Equal(30, state.Age);
    }

    [Fact]
    public void Date_uses_the_iso_format_and_sends_null_when_cleared()
    {
        var state = new ProfileState();
        var cut = Render<FbInputDate<SetBirthAction>>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Value, state.Birth));

        Assert.Equal("date", cut.Find("input").GetAttribute("type"));
        Assert.Equal("1990-05-17", cut.Find("input").GetAttribute("value"));

        cut.Find("input").Change("2001-02-03");
        Assert.Equal(new DateOnly(2001, 2, 3), state.Birth);

        cut.Find("input").Change("");
        Assert.Null(state.Birth);
    }
}
