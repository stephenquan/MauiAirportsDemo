using Microsoft.Extensions.Logging;

namespace MauiSortFilterDemo;

#pragma warning disable CS1591 // Suppress missing XML comment for publicly visible type or member

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug();
#endif

		builder.Services.AddTransient<MainPage>();
		builder.Services.AddTransient<AirportsPage>();

		Routing.RegisterRoute(nameof(AirportsPage), typeof(AirportsPage));

		return builder.Build();
	}
}
