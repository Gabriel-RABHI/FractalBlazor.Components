using FractalBlazor.WebSite.Flags;
using FractalBlazor.WebSite.StateModel;

namespace FractalBlazor.Components.Tests.Site;

/// <summary>
/// The flag assistant of the site reads the flags on the components: each one must be
/// explained by its grammar, so that a new flag never goes unnoticed.
/// </summary>
public sealed class FlagCatalogTests
{
    private static readonly ComponentFlagCatalog Catalog = new();

    private static FlagInfo Flag(string name, string component)
        => Assert.Single(Catalog.Flags, flag => flag.Name == name && flag.Components.Contains(component));

    [Fact]
    public void Every_flag_of_the_libraries_is_explained()
    {
        Assert.Empty(Catalog.Unexplained);
    }

    [Theory]
    [InlineData("PHS", "FbRow", "Padding → Horizontal → Small")]
    [InlineData("PL", "FbRow", "Padding → Large")]
    [InlineData("PLS", "FbRow", "Padding → Left → Small")]
    [InlineData("JVC", "FbGridContainer", "Justify → Vertical → Center")]
    [InlineData("JVCSE", "FbGridContainer", "Justify → Vertical → Content → Space evenly")]
    [InlineData("VO_XS", "FbStack", "Visible → Over → Extra small (512 px)")]
    [InlineData("WFL", "FbButton", "With → Frame → Light")]
    [InlineData("M", "FbText", "Medium")]
    [InlineData("Top", "FbDrawer", "Slides from the top")]
    [InlineData("Top", "FbCheckbox", "Box at the top of the content")]
    public void A_flag_reads_part_by_part_in_the_family_of_its_component(string name, string component, string meaning)
    {
        Assert.Equal(meaning, Flag(name, component).Meaning);
    }

    [Fact]
    public void A_flag_lists_the_components_that_accept_it()
    {
        var padding = Flag("PS", "FbRow");

        Assert.Contains("FbStack", padding.Components);
        Assert.Contains("FbButton", padding.Components);
        Assert.DoesNotContain("FbText", padding.Components);
    }

    [Fact]
    public void A_generic_component_is_named_as_in_the_markup()
    {
        Assert.Contains("FbCheckbox", Catalog.Components);
        Assert.DoesNotContain(Catalog.Components, component => component.Contains('`'));
    }

    [Fact]
    public void A_state_parameter_is_not_a_flag()
    {
        Assert.DoesNotContain(Catalog.Flags, flag => flag.Name == "IsOpen");
    }
}
