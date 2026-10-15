using FractalBlazor.Components.Forms;
using FractalBlazor.Components.Forms.Components.Display;
using Microsoft.Extensions.DependencyInjection;
using K =FractalBlazor.Components.Forms.Components.Display.FbCodeTokenKind;

namespace FractalBlazor.Components.Tests.Forms;

/// <summary>
/// The code block: code cut into colored pieces by its language, and a button that copies it.
/// </summary>
public sealed class CodeBlockTests : BunitContext
{
    public CodeBlockTests() => Services.AddFractalBlazor();

    private static (FbCodeTokenKind, string)[] Pieces(string code, FbCodeLanguage language)
        => FbCodeTokenizer.Tokenize(code, language).Select(token => (token.Kind, token.Text)).ToArray();

    // ------------------------------------------------------------------ Tokens

    public static TheoryData<string, FbCodeLanguage> Samples => new()
    {
        { "<FbRow NoFlex GS>\n    <FbText S B>@Item.Name</FbText>\n</FbRow>\n\n@code {\n    private int Count => 3;\n}", FbCodeLanguage.Razor },
        { "<FbSlider TValue=\"int\" Value=\"@(x ? \"a\" : \"b\")\"\n          Max=\"100\" />", FbCodeLanguage.Razor },
        { "<FbRow Unclosed=\"oops", FbCodeLanguage.Razor },
        { "@* never closed", FbCodeLanguage.Razor },
        { "var s = $\"{a}\" + @\"c:\\\" + \"\"\"raw\"\"\" + 'x' + \"unclosed", FbCodeLanguage.CSharp },
        { "/* open", FbCodeLanguage.CSharp },
        { ".fb-row:hover { color: var(--fb-x); }\ngrid-template-columns: repeat(6, minmax(0, 1fr));\n/* A */ gap: 1rem", FbCodeLanguage.Css },
        { "anything at all <b> @x", FbCodeLanguage.Plain }
    };

    [Theory]
    [MemberData(nameof(Samples))]
    public void The_pieces_join_back_into_the_code_and_two_neighbors_differ(string code, FbCodeLanguage language)
    {
        var tokens = FbCodeTokenizer.Tokenize(code, language);

        Assert.Equal(code, string.Concat(tokens.Select(token => token.Text)));
        Assert.All(tokens.Zip(tokens.Skip(1)), pair => Assert.NotEqual(pair.First.Kind, pair.Second.Kind));
    }

    [Fact]
    public void Razor_colors_tags_components_attributes_and_transitions()
    {
        Assert.Equal(
        [
            (K.Punctuation, "<"), (K.Type, "FbText"), (K.Plain, " "), (K.Attribute, "S"), (K.Plain, " "), (K.Attribute, "Title"),
            (K.Punctuation, "="), (K.String, "\"Nom\""), (K.Punctuation, ">"), (K.Razor, "@Item.Name"), (K.Plain, " ok"),
            (K.Punctuation, "</"), (K.Type, "FbText"), (K.Punctuation, "><"), (K.Tag, "br"), (K.Punctuation, "/>")
        ], Pieces("<FbText S Title=\"Nom\">@Item.Name ok</FbText><br/>", FbCodeLanguage.Razor));
    }

    [Fact]
    public void A_razor_line_without_markup_is_csharp_and_a_directive_goes_on_in_csharp()
    {
        Assert.Equal(
        [
            (K.Razor, "@foreach"), (K.Plain, " "), (K.Punctuation, "("), (K.Keyword, "var"), (K.Plain, " item "), (K.Keyword, "in"),
            (K.Plain, " "), (K.Type, "Items"), (K.Punctuation, ")"), (K.Plain, "\n"), (K.Keyword, "private"), (K.Plain, " "),
            (K.Type, "List"), (K.Punctuation, "<"), (K.Keyword, "int"), (K.Punctuation, ">"), (K.Plain, " "), (K.Function, "Load"),
            (K.Punctuation, "()"), (K.Plain, " "), (K.Punctuation, "=>"), (K.Plain, " "), (K.Number, "3"), (K.Punctuation, ";")
        ], Pieces("@foreach (var item in Items)\nprivate List<int> Load() => 3;", FbCodeLanguage.Razor));
    }

    [Fact]
    public void An_attribute_knows_the_element_it_is_written_on()
    {
        var attributes = FbCodeTokenizer.Tokenize("<FbGridContainer JHCC Columns=\"3\">\n    <div class=\"x\"></div>", FbCodeLanguage.Razor)
            .Where(token => token.Kind == K.Attribute)
            .Select(token => (token.Text, token.Element));

        Assert.Equal([("JHCC", "FbGridContainer"), ("Columns", "FbGridContainer"), ("class", "div")], attributes);
    }

    [Fact]
    public void An_attribute_expression_keeps_its_own_quotes()
    {
        var pieces = Pieces("<FbBadge Title=\"@(on ? \"En ligne\" : \"Hors ligne\")\" Dot />", FbCodeLanguage.Razor);

        Assert.Contains((K.String, "\"@(on ? \"En ligne\" : \"Hors ligne\")\""), pieces);
        Assert.Contains((K.Attribute, "Dot"), pieces);
    }

    [Fact]
    public void Comments_are_comments()
    {
        Assert.Equal([(K.Comment, "@* note *@"), (K.Plain, "\n"), (K.Comment, "<!-- html -->")],
            Pieces("@* note *@\n<!-- html -->", FbCodeLanguage.Razor));
        Assert.Equal([(K.Keyword, "int"), (K.Plain, " "), (K.Comment, "// count")], Pieces("int // count", FbCodeLanguage.CSharp));
    }

    [Fact]
    public void Csharp_knows_its_strings_numbers_and_calls()
    {
        var pieces = Pieces("Handle(\"a\\\"b\", @\"c:\\\", $\"{x}\", 'y', 3.14f, \"\"\"raw\"\"\");", FbCodeLanguage.CSharp);

        Assert.Equal((K.Function, "Handle"), pieces[0]);
        Assert.Contains((K.String, "\"a\\\"b\""), pieces);
        Assert.Contains((K.String, "@\"c:\\\""), pieces);
        Assert.Contains((K.String, "$\"{x}\""), pieces);
        Assert.Contains((K.String, "'y'"), pieces);
        Assert.Contains((K.Number, "3.14f"), pieces);
        Assert.Contains((K.String, "\"\"\"raw\"\"\""), pieces);
    }

    [Fact]
    public void Css_knows_selectors_properties_functions_and_variables()
    {
        Assert.Equal(
        [
            (K.Selector, ".fb-row:hover"), (K.Plain, " "), (K.Punctuation, "{"), (K.Plain, " "), (K.Property, "color"),
            (K.Punctuation, ":"), (K.Plain, " "), (K.Function, "var"), (K.Punctuation, "("), (K.Variable, "--fb-x"),
            (K.Punctuation, ");"), (K.Plain, " "), (K.Punctuation, "}"), (K.Plain, "\n"), (K.Comment, "/* A */"), (K.Plain, " "),
            (K.Property, "gap"), (K.Punctuation, ":"), (K.Plain, " "), (K.Function, "repeat"), (K.Punctuation, "("), (K.Number, "6"),
            (K.Punctuation, ","), (K.Plain, " "), (K.Number, "1fr"), (K.Punctuation, ");")
        ], Pieces(".fb-row:hover { color: var(--fb-x); }\n/* A */ gap: repeat(6, 1fr);", FbCodeLanguage.Css));
    }

    // ------------------------------------------------------------------ Block

    [Fact]
    public void The_block_is_a_surface_that_shows_its_colored_code()
    {
        var root = Render<FbCodeBlock>(p => p.Add(x => x.Code, "<FbRow GS />")).Find(".fb-code-block");

        Assert.Contains("fb-wba", root.ClassList);
        Assert.Equal("<FbRow GS />", root.QuerySelector("pre.fb-code-pre > code")!.TextContent);
        Assert.Equal(["fb-code-p", "fb-code-t", "fb-code-a", "fb-code-p"],
            root.QuerySelectorAll("code > span").Select(span => span.ClassName));
        Assert.Equal("0", root.QuerySelector("pre")!.GetAttribute("tabindex"));
    }

    private sealed class Glossary : IFbCodeGlossary
    {
        public string? Explain(string element, string attribute)
            => (element, attribute) == ("FbGridContainer", "JHCC") ? "Justify → Horizontal → Content → Center" : null;
    }

    [Fact]
    public void A_registered_glossary_gives_its_attributes_a_bubble()
    {
        Services.AddSingleton<IFbCodeGlossary>(new Glossary());

        var spans = Render<FbCodeBlock>(p => p.Add(x => x.Code, "<FbGridContainer JHCC GS />")).FindAll("code > span.fb-code-a");

        Assert.Equal("Justify → Horizontal → Content → Center", spans[0].GetAttribute("data-fb-tip"));
        Assert.False(spans[1].HasAttribute("data-fb-tip"));
    }

    [Fact]
    public void Without_a_glossary_no_attribute_has_a_bubble()
    {
        var cut = Render<FbCodeBlock>(p => p.Add(x => x.Code, "<FbGridContainer JHCC />"));

        Assert.Empty(cut.FindAll("[data-fb-tip]"));
    }

    [Fact]
    public void A_language_flag_changes_the_colors_and_falls_back_once_removed()
    {
        var cut = Render<FbCodeBlock>(p => p.Add(x => x.Code, "<b>").Add(x => x.Plain, true));
        Assert.Empty(cut.FindAll("code > span"));

        cut.Render(p => p.Add(x => x.Code, "<b>"));
        Assert.Equal(FbCodeLanguage.Razor, cut.Instance.Language);
        Assert.NotEmpty(cut.FindAll("code > span"));
    }

    [Fact]
    public void The_button_copies_the_code_and_says_so()
    {
        JSInterop.Setup<bool>("FractalBlazor.copyText", "int x;").SetResult(true);
        var cut = Render<FbCodeBlock>(p => p.Add(x => x.Code, "int x;").Add(x => x.CSharp, true)
            .Add(x => x.CopyTitle, "Copier").Add(x => x.CopiedTitle, "Copié"));
        Assert.Equal("Copier", cut.Find(".fb-code-copy button").GetAttribute("title"));

        cut.Find(".fb-code-copy button").Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("fb-code-copied", cut.Find(".fb-code-block").ClassList);
            Assert.Equal("Copié", cut.Find(".fb-code-copy button").GetAttribute("title"));
            Assert.Equal("Copié", cut.Find(".fb-code-status[role=status]").TextContent);
        });
    }

    [Fact]
    public void A_refused_copy_says_nothing()
    {
        JSInterop.Setup<bool>("FractalBlazor.copyText", "x").SetResult(false);
        var cut = Render<FbCodeBlock>(p => p.Add(x => x.Code, "x"));

        cut.Find(".fb-code-copy button").Click();

        Assert.DoesNotContain("fb-code-copied", cut.Find(".fb-code-block").ClassList);
        Assert.Equal("", cut.Find(".fb-code-status").TextContent);
    }

    [Fact]
    public void NoCopy_has_no_button()
    {
        var cut = Render<FbCodeBlock>(p => p.Add(x => x.Code, "x").Add(x => x.NoCopy, true));

        Assert.Empty(cut.FindAll(".fb-code-copy"));
        Assert.Empty(cut.FindAll(".fb-code-status"));
    }
}
