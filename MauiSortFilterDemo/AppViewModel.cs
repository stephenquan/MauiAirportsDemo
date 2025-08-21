using System.Collections.ObjectModel;
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
	public partial ObservableCollection<Airport> AirportSearchResults { get; set; } = [];

	/// <summary>
	/// Initializes a new instance of the <see cref="AppViewModel"/> class.
	/// </summary>
	public AppViewModel()
	{
		Logger?.LogDebug($"{DateTime.Now} AppViewModel constructor called, initializing in-memory SQLite database.");
		Db = new SQLiteConnection(":memory:");
	}

	long searchRequestTime = 0;
	bool searchInProgress = false;

	/// <summary>
	/// Executes a search for airports based on the current search text.
	/// </summary>
	public async Task ExecuteAirportSearchAsync()
	{
		searchRequestTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();

		if (!searchInProgress)
		{
			searchInProgress = true;
			while (DateTimeOffset.Now.ToUnixTimeMilliseconds() < searchRequestTime + 250)
			{
				await Task.Delay(50);
			}
			string currentSearchText = AirportSearchText;

			Logger?.LogDebug($"{DateTime.Now} Executing airport search with text: '{currentSearchText}'");
			AirportSearchResults.Clear();
			var newResults = await Task.Run(() => Db.Query<Airport>(
				"SELECT * FROM Airport WHERE Name LIKE ? ORDER BY Name LIMIT 201",
				$"%{currentSearchText}%"));
			AirportSearchResults.Clear();
			foreach (var result in newResults)
			{
				AirportSearchResults.Add(result);
			}
			Logger?.LogDebug($"{DateTime.Now} Airport search completed, found {AirportSearchResults.Count} results.");

			searchInProgress = false;

			if (currentSearchText != AirportSearchText)
			{
				Logger?.LogDebug($"{DateTime.Now} Search request was invalidated, re-executing search.");
				await ExecuteAirportSearchAsync();
			}
		}
	}

	/// <summary>
	/// Populates the SQLite database with airport data from a predefined text file.
	/// </summary>
	/// <returns></returns>
	public async Task PopulateAirportsAsync()
	{
		Logger?.LogDebug($"{DateTime.Now} Populating airports in the SQLite database.");
		Db.BeginTransaction();
		Db.DropTable<Airport>();
		Db.CreateTable<Airport>();
		int count = 0;
		int errors = 0;
		using (var stream = await FileSystem.OpenAppPackageFileAsync("airports_83520.csv"))
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
					//Logger?.LogError(ex, $"{DateTime.Now} Error inserting airport {airport.Name} into the database: {ex.Message}");
					errors = 0;
				}
			}
		}
		Db.Commit();
		Logger?.LogDebug($"{DateTime.Now} Airports populated in the SQLite database with {count} records and {errors} errors.");
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
