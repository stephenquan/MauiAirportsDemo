using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper;
using Microsoft.Extensions.Logging;
using SQLite;

namespace MauiSortFilterDemo;

/// <summary>
/// Represents the main view model for the application, providing access to the SQLite database and managing
/// application-level data and initialization tasks.
/// </summary>
public partial class AppViewModel : ObservableObject
{
	/// <summary>
	/// Gets the logger instance used to log messages for the application.
	/// </summary>
	public static ILogger? Logger { get; } = IPlatformApplication.Current?.Services.GetService<ILogger<AppViewModel>>();

	/// <summary>
	/// Gets the SQLite database connection.
	/// </summary>
	public SQLiteConnection Db { get; }

	/// <summary>
	/// Gets or sets the text used to search for airports.
	/// </summary>
	[ObservableProperty]
	public partial string AirportSearchText { get; set; } = "M";

	/// <summary>
	/// Gets a list of airports filtered by the search text.
	/// </summary>
	[ObservableProperty]
	public partial List<Airport> AirportSearchResults { get; set; } = [];

	/// <summary>
	/// Initializes a new instance of the <see cref="AppViewModel"/> class.
	/// </summary>
	public AppViewModel()
	{
		Logger?.LogDebug($"{DateTime.Now} AppViewModel constructor called, initializing in-memory SQLite database.");
		Db = new SQLiteConnection(":memory:");
		this.PropertyChanged += (s, e) =>
		{
			switch (e.PropertyName)
			{
				case nameof(AirportSearchText):
					ExecuteAirportSearch();
					break;
			}
		};
	}

	/// <summary>
	/// Executes a search for airports based on the current search text.
	/// </summary>
	public void ExecuteAirportSearch()
	{
		Logger?.LogDebug($"{DateTime.Now} Executing airport search with text: '{AirportSearchText}'");
		AirportSearchResults = Db.Query<Airport>(
			"SELECT * FROM Airport WHERE Name LIKE ? ORDER BY Name LIMIT 201",
			$"{AirportSearchText}%");
		Logger?.LogDebug($"{DateTime.Now} Airport search completed, found {AirportSearchResults.Count} results.");
	}

	/// <summary>
	/// Populates the SQLite database with airport data from a predefined text file.
	/// </summary>
	/// <returns></returns>
	public async Task PopulateAirports()
	{
		Logger?.LogDebug($"{DateTime.Now} Populating airports in the SQLite database.");
		Db.BeginTransaction();
		Db.DropTable<Airport>();
		Db.CreateTable<Airport>();
		int count = 0;
		using (var stream = await FileSystem.OpenAppPackageFileAsync("airports.csv"))
		using (var reader = new StreamReader(stream))
		using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
		{
			var airports = csv.GetRecords<Airport>();
			foreach (var airport in airports)
			{
				try
				{
					Db.Insert(airport);
					count++;
				}
				catch (Exception ex)
				{
					Logger?.LogError(ex, $"{DateTime.Now} Error inserting airport {airport.Name} into the database: {ex.Message}");
				}
			}
		}
		Db.Commit();
		Logger?.LogDebug($"{DateTime.Now} Airports populated in the SQLite database with {count} records.");
	}

	/// <summary>
	/// Invalidates the current airport search results and notifies listeners of the change.
	/// </summary>
	public void InvalidateAirportSearchResults()
	{
		Logger?.LogDebug($"{DateTime.Now} Invalidating AirportSearchResults due to change in AirportSearchText.");
		OnPropertyChanged(nameof(AirportSearchResults));
	}
}
