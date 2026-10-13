using static FractalBlazor.WebSite.StateModel.Tests.TestFlagCatalog;

namespace FractalBlazor.WebSite.StateModel.Tests;

/// <summary>
/// The flag assistant is driven by its actions alone: typed letters, chosen parts, a
/// component search.
/// </summary>
public sealed class FlagAssistantTests
{
    private readonly FlagAssistantState _assistant = new(new TestFlagCatalog());

    private void Type(string query) => _assistant.Handle(new FlagAssistantState.SetQuery(query));

    private void Choose(int column, FlagToken token) => _assistant.Handle(new FlagAssistantState.ChooseToken(column, token));

    private IEnumerable<string> Matches => _assistant.Matches.Select(flag => flag.Name);

    private FlagChoiceState State(int column, FlagToken token)
        => _assistant.Columns[column].Groups.SelectMany(group => group.Choices).Single(choice => choice.Token == token).State;

    private IEnumerable<string> Categories(int column) => _assistant.Columns[column].Groups.Select(group => group.Category);

    [Fact]
    public void Empty_it_shows_how_every_flag_starts()
    {
        var first = Assert.Single(_assistant.Columns);

        Assert.Equal(["Spacing", "Size", "Weight", "Layout"], first.Groups.Select(group => group.Category));
        Assert.All(first.Groups.SelectMany(group => group.Choices), choice => Assert.Equal(FlagChoiceState.Possible, choice.State));
        Assert.Equal(9, _assistant.Matches.Count);
        Assert.Equal(["FbButton", "FbRow", "FbRowItem", "FbStack", "FbText"], _assistant.Components);
    }

    [Fact]
    public void A_letter_selects_its_part_and_opens_the_next_column_grouped_by_kind()
    {
        Type("P");

        Assert.Equal(FlagChoiceState.Selected, State(0, Padding));
        Assert.Equal(FlagChoiceState.Other, State(0, Margin));
        Assert.Equal(["Spacing", "Direction"], Categories(1));
        Assert.Equal(FlagChoiceState.Possible, State(1, Left));
        Assert.Equal(["PS", "PL", "PHS", "PLS", "PLL"], Matches);
    }

    [Fact]
    public void Letters_that_read_two_ways_select_both_parts()
    {
        Type("PL");

        Assert.Equal(FlagChoiceState.Selected, State(1, Large));
        Assert.Equal(FlagChoiceState.Selected, State(1, Left));
        Assert.Equal(3, _assistant.Columns.Count);
        Assert.Equal(["PL", "PLS", "PLL"], Matches);
    }

    [Fact]
    public void More_letters_decide_between_the_readings()
    {
        Type("PLS");

        Assert.Equal(FlagChoiceState.Selected, State(1, Left));
        Assert.Equal(FlagChoiceState.Other, State(1, Large));
        Assert.Equal(FlagChoiceState.Selected, State(2, Small));
        Assert.Equal(FlagChoiceState.Other, State(2, Large));
        Assert.Equal(["PLS"], Matches);
    }

    [Fact]
    public void Choosing_a_part_spells_it_and_choosing_it_again_removes_it()
    {
        Choose(0, Padding);
        Choose(1, Left);
        Assert.Equal("PL", _assistant.Query);

        Choose(2, Small);
        Assert.Equal("PLS", _assistant.Query);

        Choose(1, Left);
        Assert.Equal("P", _assistant.Query);
    }

    [Fact]
    public void Typed_letters_ignore_case_and_chosen_parts_spell_the_flag()
    {
        Type("pl");
        Assert.Equal(["PL", "PLS", "PLL"], Matches);

        Choose(2, Small);
        Assert.Equal("PLS", _assistant.Query);
    }

    [Fact]
    public void Letters_that_lead_nowhere_keep_their_path_selected()
    {
        Type("PQ");

        Assert.Empty(_assistant.Matches);
        Assert.Equal(FlagChoiceState.Selected, State(0, Padding));
        Assert.Equal(FlagChoiceState.Other, State(1, Small));
    }

    [Fact]
    public void Part_of_a_component_name_narrows_the_flags_to_its_components()
    {
        _assistant.Handle(new FlagAssistantState.SetComponentQuery("text"));

        Assert.Equal(["FbText"], _assistant.Components);
        Assert.Equal(["M", "B"], Matches);
        Assert.Equal(FlagChoiceState.Possible, State(0, Medium));
        Assert.Equal(["Size", "Weight"], Categories(0));
    }

    [Fact]
    public void A_chosen_component_keeps_its_own_flags_only()
    {
        _assistant.Handle(new FlagAssistantState.SetComponentQuery("Row"));
        Assert.Equal(["FbRow", "FbRowItem"], _assistant.Components);

        _assistant.Handle(new FlagAssistantState.ChooseComponent("FbRowItem"));

        Assert.Equal("FbRowItem", _assistant.ComponentQuery);
        Assert.Equal(["FbRowItem"], _assistant.Components);
        Assert.Equal(["MS"], Matches);

        _assistant.Handle(new FlagAssistantState.ChooseComponent("FbRowItem"));
        Assert.Equal("", _assistant.ComponentQuery);
    }

    [Fact]
    public void The_components_follow_the_flag_field()
    {
        Type("PLS");

        Assert.Equal(["FbRow", "FbStack"], _assistant.Components);
    }

    [Fact]
    public void A_flag_of_the_list_fills_the_field_and_the_cross_clears_it()
    {
        _assistant.Handle(new FlagAssistantState.ChooseFlag("PHS"));
        Assert.Equal("PHS", _assistant.Query);
        Assert.Equal(FlagChoiceState.Selected, State(1, Horizontal));

        _assistant.Handle(new FlagAssistantState.ClearQuery());
        Assert.Equal("", _assistant.Query);
        Assert.Single(_assistant.Columns);
    }

    [Fact]
    public void Only_a_change_notifies_the_view()
    {
        var notifications = 0;
        _assistant.OnStateChanged += () => notifications++;

        Type("P");
        Type("P");
        Type(" P ");

        Assert.Equal(1, notifications);
    }
}
