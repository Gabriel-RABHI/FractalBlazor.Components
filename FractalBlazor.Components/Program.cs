using FractalBlazor.Components;
using FractalBlazor.Components.Components;
using FractalBlazor.Components.Components.ThemeEditor;
using FractalBlazor.Components.Forms;
using FractalBlazor.Demo.States;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
// Theme registries, and a view registry for every selector (ViewSelector, FbViewSelector…).
builder.Services.AddFractalBlazor();
// One application state per circuit: the views only display it and send it actions.
builder.Services.AddScoped<RootState>();
builder.Services.AddScoped<ThemeEditorModel>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
