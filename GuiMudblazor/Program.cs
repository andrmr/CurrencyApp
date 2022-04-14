using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GuiMudblazor;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

builder.Services.AddSingleton<ICurrencyService>(sp => ActivatorUtilities.CreateInstance<CurrencyService>(sp, sp.GetRequiredService<IConfiguration>(), CurrencyServiceType.Fiat));
builder.Services.AddSingleton<ICurrencyService>(sp => ActivatorUtilities.CreateInstance<CurrencyService>(sp, sp.GetRequiredService<IConfiguration>(), CurrencyServiceType.Crypto));

await builder.Build().RunAsync();
