using FractalBlazor.Components.Layout;

namespace FractalBlazor.Components.Tests.Layout;

public sealed class ShowWhenVisibleTests : BunitContext
{
    private readonly BunitJSModuleInterop _observer;

    public ShowWhenVisibleTests()
    {
        _observer = JSInterop.SetupModule(invocation => invocation.Identifier == "FractalBlazor.observeVisibility");
        _observer.SetupVoid("dispose");
    }

    [Fact]
    public void Shows_the_spinner_until_visible_then_the_content()
    {
        var calls = 0;
        var cut = Render<FbShowWhenVisible>(p => p
            .Add(x => x.DoWhenVisible, () => calls++)
            .AddChildContent("<span class=\"content\">Loaded</span>"));

        Assert.NotEmpty(cut.FindAll(".fb-prog-loader"));
        Assert.Empty(cut.FindAll(".content"));
        Assert.Single(JSInterop.Invocations, i => i.Identifier == "FractalBlazor.observeVisibility");

        cut.InvokeAsync(() => cut.Instance.OnVisibilityChanged(true));

        Assert.NotEmpty(cut.FindAll(".content"));
        Assert.Equal(1, calls);
        _observer.VerifyInvoke("dispose");
    }

    [Fact]
    public void Leaving_the_viewport_keeps_the_content_without_reset()
    {
        var cut = Render<FbShowWhenVisible>(p => p.AddChildContent("<span class=\"content\">Loaded</span>"));

        cut.InvokeAsync(() => cut.Instance.OnVisibilityChanged(true));
        cut.InvokeAsync(() => cut.Instance.OnVisibilityChanged(false));

        Assert.NotEmpty(cut.FindAll(".content"));
    }

    [Fact]
    public void Reset_goes_back_to_waiting_when_leaving_the_viewport()
    {
        var cut = Render<FbShowWhenVisible>(p => p
            .Add(x => x.Reset, true)
            .Add(x => x.WaitContent, "<span class=\"wait\">Wait</span>")
            .AddChildContent("<span class=\"content\">Loaded</span>"));

        cut.InvokeAsync(() => cut.Instance.OnVisibilityChanged(true));
        Assert.NotEmpty(cut.FindAll(".content"));

        cut.InvokeAsync(() => cut.Instance.OnVisibilityChanged(false));
        Assert.NotEmpty(cut.FindAll(".wait"));
        Assert.Empty(cut.FindAll(".content"));
    }
}
