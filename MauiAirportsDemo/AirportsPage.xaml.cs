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
	/// Gets or setts whether the Melbourne MBR filter is applied.
	/// </summary>
	[BindableProperty]
	[NotifyPropertyChangedFor(nameof(Results))]
	public partial bool InsideMelbourneMBR { get; set; } = false;

	/// <summary>
	/// Gets or sets whether the Los Angeles MBR filter is applied.
	/// </summary>
	[BindableProperty]
	[NotifyPropertyChangedFor(nameof(Results))]
	public partial bool InsideLosAngelesMBR { get; set; } = false;

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
			string fromClause2 = "";
			string whereClause = "WHERE Length(JsonProperty(a.Properties, 'iata')) > 0 ";
			string orderClause = "";
			string limitClause = "LIMIT 100";

			orderClause = $@"
ORDER BY JsonProperty(a.Properties, 'iata'),
         a.Name
";

			if (!string.IsNullOrEmpty(SearchText))
			{
				whereClause += $@"
AND (a.Name LIKE '{SearchText}%'
     OR JsonProperty(a.Properties, 'iata') LIKE '%{SearchText}%')
";
			}

			if (InsideMelbourneMBR)
			{
				fromClause2 = ", Airports_RTree r";
				whereClause += $@"
AND r.MinX <= 145.5 AND r.MaxX >= 144.4
AND r.MinY <= -37.5 AND r.MaxY >= -38.3
AND a.Id = r.Id
";
			}

			if (InsideLosAngelesMBR)
			{
				fromClause2 = ", Airports_RTree r";
				whereClause += $@"
AND r.MinX <= -116.8667 AND r.MaxX >= -119.0833
AND r.MinY <= 34.8233 AND r.MaxY >= 33.4333
AND a.Id = r.Id
";
			}

			return db.Query<AirportSQLData>($@"
{selectClause}
{fromClause}
{fromClause2}
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
							Latitude = csvData.Latitude,
							IATA = csvData.IATA
						}
					});
				}
			}));

		// Verify data has been loaded.
		var count = db.ExecuteScalar<int>("SELECT COUNT(*) FROM Airports");
		Logger?.LogInformation($"Database populated with {count} airport records.");

		// Finalize the initialization.
		IsLoading = false;
	}
}
