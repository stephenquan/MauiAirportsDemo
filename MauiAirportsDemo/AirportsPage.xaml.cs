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
	[NotifyPropertyChangedFor(nameof(Results))]
	public partial bool IsLoading { get; private set; } = true;

	/// <summary>
	/// Gets whether the Melbourne MBR filter is applied.
	/// </summary>
	[BindableProperty]
	[NotifyPropertyChangedFor(nameof(Results))]
	public partial bool MelbourneMBR { get; set; } = false;

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
	{
		get
		{
			if (IsLoading)
			{
				return new();
			}

			string selectClause = "SELECT a.*";
			string fromClause = "FROM Airports a";
			string whereClause = "WHERE 1 = 1";
			string orderClause = "";
			string limitClause = "LIMIT 100";

			if (!string.IsNullOrEmpty(SearchText))
			{
				whereClause += $" AND a.Name LIKE '{SearchText}%'";
				orderClause = "ORDER BY a.Name";
			}

			if (MelbourneMBR)
			{
				fromClause += ", Airports_RTree r";
				whereClause += $@"
AND r.MinX <= 145.5 AND r.MaxX >= 144.4
AND r.MinY <= -37.5 AND r.MaxY >= -38.3
AND a.Id = r.Id
";
				orderClause = "ORDER BY a.Name";
			}

			return db.Query<AirportSQLData>($@"
{selectClause}
{fromClause}
{whereClause}
{orderClause}
{limitClause}
;");
		}
	}

	/// <summary>
	/// The SQLite database connection.
	/// </summary>
	SQLiteConnection db { get; } = new(":memory:");

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
		db.CreateJsonFunctions();
		await db.LoadSchemaFromMauiAsset("schema.sql");
		var csvRecords = await CsvExtensions.LoadCSVFromMauiAsset<AirportCSVData>("airports_83520.csv");
		await Task.Run(() =>
			db.RunInTransaction(() =>
			{
				foreach (var csvData in csvRecords)
				{
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
					});
				}
			}));

		// Verify data has been loaded.
		var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM Airports");
		Logger?.LogInformation($"Database populated with {count} airport records.");

		// Test the R*Tree spatial index.
		var search = db.Query<AirportSQLData>(@"
SELECT a.*
FROM   Airports a
JOIN   Airports_RTree r ON a.Id = r.Id
WHERE  r.MinX <= 145.5 AND r.MaxX >= 144.4
AND    r.MinY <= -37.5 AND r.MaxY >= -38.3
ORDER BY a.Name
;");
		Logger?.LogInformation($"R*Tree spatial search found {search.Count} airports in Greater Melbourne (Victoria, Australia).");
		foreach (var airport in search)
		{
			Logger?.LogInformation($"Found airport in R*Tree search: {airport.Name} ({airport.Properties?.Ident})");
		}

		// Finalize the initialization.
		IsLoading = false;
	}
}
