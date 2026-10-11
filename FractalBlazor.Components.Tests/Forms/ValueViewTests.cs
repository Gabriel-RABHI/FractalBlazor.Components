using System.ComponentModel.DataAnnotations;
using System.Globalization;
using FractalBlazor.Components.Forms;
using FractalBlazor.Components.Forms.Contracts;
using FractalBlazor.Components.Forms.Mapping;
using FractalBlazor.Components.Layout.Abstracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.Extensions.DependencyInjection;

namespace FractalBlazor.Components.Tests.Forms;

public enum Priority
{
    Low,
    [Display(Name = "Très haute")]
    VeryHigh
}

public sealed class ShoutedText : FbComponentBase
{
    [Parameter]
    public object? Item { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
        => builder.AddContent(0, ((string)Item!).ToUpperInvariant());
}

public sealed class ValueViewTests : BunitContext
{
    public ValueViewTests()
    {
        Services.AddFractalBlazor();
    }

    private string TextOf(object item)
        => Render<FbDynamicView<FbViewSelector>>(p => p.Add(x => x.Item, item)).Markup;

    [Fact]
    public void Strings_are_rendered_without_a_declared_view()
    {
        Assert.Contains(">Paris</span>", TextOf("Paris"));
    }

    [Fact]
    public void Enumerations_use_their_display_name()
    {
        Assert.Contains(">Low</span>", TextOf(Priority.Low));
        Assert.Contains(">Très haute</span>", TextOf(Priority.VeryHigh));
    }

    [Fact]
    public void Numbers_and_dates_use_the_current_culture()
    {
        var culture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            Assert.Equal("1,5", FbValueView.TextOf(1.5));
            Assert.Equal("17/05/1990", FbValueView.TextOf(new DateOnly(1990, 5, 17)));
        }
        finally
        {
            CultureInfo.CurrentCulture = culture;
        }
    }

    [Fact]
    public void A_declared_view_wins_over_the_built_in_one()
    {
        Services.GetRequiredService<IFbViewRegistry<FbViewSelector>>().Register<string, ShoutedText>(FbViewSelector.Item);

        Assert.Contains("PARIS", TextOf("Paris"));
    }

    [Fact]
    public void Objects_without_a_view_are_still_reported()
    {
        Assert.Contains("No Item view registered for type Version", TextOf(new Version(1, 0)));
    }

    [Fact]
    public void Every_selector_gets_a_registry()
    {
        Assert.NotNull(Services.GetService<IFbViewRegistry<TestSelector>>());
        Assert.NotNull(Services.GetService<IFbViewRegistry<FbViewSelector>>());
    }
}
