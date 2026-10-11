using FractalBlazor.WebSite;
using FractalBlazor.WebSite.ClientSide;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// FractalBlazor and the states of the site: one application state for the page.
builder.Services.AddFractalBlazorWebSite();

await builder.Build().RunAsync();
