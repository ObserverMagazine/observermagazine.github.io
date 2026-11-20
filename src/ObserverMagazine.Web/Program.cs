using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ObserverMagazine.Web;
using ObserverMagazine.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddObserverMagazine(new Uri(builder.HostEnvironment.BaseAddress));
builder.Logging.SetMinimumLevel(LogLevel.Information);

await builder.Build().RunAsync();
