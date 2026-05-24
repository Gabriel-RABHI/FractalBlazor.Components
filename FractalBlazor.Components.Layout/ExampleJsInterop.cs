using Microsoft.JSInterop;

namespace FractalBlazor.Components.Layout;

// This class provides an example of how JavaScript functionality can be wrapped
// in a .NET class for easy consumption. The associated JavaScript module is
// loaded on demand when first needed.
//
// This class can be registered as scoped DI service and then injected into Blazor
// components for use.

public class ExampleJsInterop(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/FractalBlazor.Components.Layout/exampleJsInterop.js").AsTask());

    public async ValueTask<string> Prompt(string message)
    {
        var module = await moduleTask.Value;
        return await module.InvokeAsync<string>("showPrompt", message);
    }

    public async ValueTask DisposeAsync()
    {
        if (moduleTask.IsValueCreated)
        {
            var module = await moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}

public static class FbSpacings {
    public static FbMargin S_Margin { get; set; } = FbMargin.Margin_8;

    public static FbPadding S_Padding { get; set; } = FbPadding.Padding_8;

    public static FbGutter S_Gutter { get; set; } = FbGutter.Gutter_8;

    public static FbMargin M_Margin { get; set; } = FbMargin.Margin_12;

    public static FbPadding M_Padding { get; set; } = FbPadding.Padding_12;

    public static FbGutter M_Gutter { get; set; } = FbGutter.Gutter_12;

    public static FbMargin L_Margin { get; set; } = FbMargin.Margin_24;

    public static FbPadding L_Padding { get; set; } = FbPadding.Padding_24;

    public static FbGutter L_Gutter { get; set; } = FbGutter.Gutter_24;
}
