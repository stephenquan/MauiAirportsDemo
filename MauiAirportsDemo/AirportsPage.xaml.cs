// AirportsPage.xaml.cs

using Microsoft.Extensions.Logging;
using SQLite;
using SQuan.Helpers.Maui.Mvvm;

namespace MauiAirportsDemo;

/// <summary>
/// Represents a page that displays airport-related content in the application.
/// </summary>
public partial class AirportsPage : ContentPage
{
	/// <summary>
	/// Gets the logger instance used to log messages for the AirportsPage.
	/// </summary>
	public static ILogger? Logger { get; } = IPlatformApplication.Current?.Services.GetService<ILogger<AirportsPage>>();

	/// <summary>
	/// Gets whether the page is currently loading data.
	/// </summary>
	[BindableProperty]
	public partial bool IsLoading { get; private set; } = true;

	SQLiteConnection db { get; } = new(":memory:");

	/// <summary>
	/// Gets or sets the search text used to filter airport results.
	/// </summary>
	[BindableProperty]
	[NotifyPropertyChangedFor(nameof(Results))]
	public partial string SearchText { get; set; } = string.Empty;

	/// <summary>
	/// Gets the list of airport results from the database.
	/// </summary>
	public List<AirportSQLData> Results
		=> IsLoading
		? []
		: string.IsNullOrEmpty(SearchText)
			? db.Query<AirportSQLData>("SELECT * FROM Airports LIMIT 100")
			: db.Query<AirportSQLData>($"SELECT * FROM Airports WHERE Name LIKE '{SearchText}%' ORDER BY Name LIMIT 100");

	/// <summary>
	/// Initializes a new instance of the <see cref="AirportsPage"/> class.
	/// </summary>
	public AirportsPage()
	{
		BindingContext = this;
		InitializeComponent();
		Dispatcher.Dispatch(async () => await PostInitialize());
	}

	async Task PostInitialize()
	{
		IsLoading = true;

		// Create and populate SQL database from CSV asset.
		await db.LoadSchemaFromMauiAsset("schema.sql");
		var csvRecords = await CsvExtensions.LoadCSVFromMauiAsset<AirportCSVData>("airports_83520.csv");
		await Task.Run(() =>
			db.RunInTransaction(() =>
			{
				csvRecords.ForEach(csvData =>
					db.Insert(new AirportSQLData
					{
						Id = csvData.Id,
						Name = csvData.Name,
						Properties = new AirportProperties()
						{
							Ident = csvData.Ident,
							Longitude = csvData.Longitude,
							Latitude = csvData.Latitude
						}
					})
				);
			}));

		// Verify data has been loaded.
		var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM Airports");
		Logger?.LogInformation($"Database populated with {count} airport records.");

		// Finalize the initialization.
		IsLoading = false;
		OnPropertyChanged(nameof(Results));
	}
}