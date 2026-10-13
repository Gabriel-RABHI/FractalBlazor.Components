using FractalBlazor.Components.Common.Contracts;
using FractalBlazor.Components.Forms.Components.Sensors;

namespace FractalBlazor.Components.Tests.Forms;

public sealed class WidthSensorTests : BunitContext
{
    public WidthSensorTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    private sealed class ScreenProbe
    {
        public sealed record SetWidth(FbWidthClass Value) : IStateAction<FbWidthClass>;

        public FbWidthClass Width { get; private set; } = FbWidthClass.XL;

        public int Calls { get; private set; }

        public void Handle(SetWidth action)
        {
            Calls++;
            Width = action.Value;
        }
    }

    private IRenderedComponent<FbWidthSensor<ScreenProbe.SetWidth>> RenderSensor(ScreenProbe state)
        => Render<FbWidthSensor<ScreenProbe.SetWidth>>(p => p
            .Add(x => x.Handler, state)
            .Add(x => x.Value, state.Width));

    [Fact]
    public void The_sensor_is_hidden_and_observes_its_parent_with_the_shared_thresholds()
    {
        var cut = RenderSensor(new ScreenProbe());

        Assert.Equal("true", cut.Find("span.fb-width-sensor").GetAttribute("aria-hidden"));
        var invocation = Assert.Single(JSInterop.Invocations, invocation => invocation.Identifier == "FractalBlazor.observeWidth");
        Assert.Equal(FbWidthClasses.Thresholds, invocation.Arguments[2]);
    }

    [Fact]
    public async Task A_new_category_is_sent_to_the_state()
    {
        var state = new ScreenProbe();
        var cut = RenderSensor(state);

        await cut.InvokeAsync(() => cut.Instance.OnWidthClassChanged((int)FbWidthClass.S));

        Assert.Equal(FbWidthClass.S, state.Width);
    }

    [Fact]
    public async Task The_category_the_state_holds_is_not_sent_again()
    {
        var state = new ScreenProbe();
        var cut = RenderSensor(state);

        await cut.InvokeAsync(() => cut.Instance.OnWidthClassChanged((int)FbWidthClass.XL));

        Assert.Equal(0, state.Calls);
    }

    [Fact]
    public async Task A_disposed_sensor_sends_nothing()
    {
        var state = new ScreenProbe();
        var cut = RenderSensor(state);
        var sensor = cut.Instance;

        await DisposeComponentsAsync();
        await sensor.OnWidthClassChanged((int)FbWidthClass.Base);

        Assert.Equal(0, state.Calls);
    }
}
