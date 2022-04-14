using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace ClientGui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();

		using var appsettingsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ClientGui.appsettings.json");
		builder.Configuration.AddJsonStream(appsettingsStream);

		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();

        builder.Services.AddSingleton<ICurrencyService>(sp => ActivatorUtilities.CreateInstance<CurrencyService>(sp, sp.GetRequiredService<IConfiguration>(), CurrencyServiceType.Fiat));
        builder.Services.AddSingleton<ICurrencyService>(sp => ActivatorUtilities.CreateInstance<CurrencyService>(sp, sp.GetRequiredService<IConfiguration>(), CurrencyServiceType.Crypto));

        return builder.Build();
	}
}
