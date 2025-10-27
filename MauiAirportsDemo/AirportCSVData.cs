// AirportCSVData.cs

using System.Text.Json.Serialization;

namespace MauiAirportsDemo;

/// <summary>
/// Represents an airport with a name.
/// </summary>
public partial class AirportCSVData
{
	/// <summary>
	/// Gets or sets the airport ID.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("id")]
	[JsonPropertyName("id")]
	public int Id { get; set; } = 0;

	/// <summary>
	/// Gets or sets the airport identifier.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("ident")]
	[JsonPropertyName("ident")]
	public string? Ident { get; set; }

	/// <summary>
	/// Gets or sets the airport type.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("type")]
	[JsonPropertyName("type")]
	public string? Type { get; set; }

	/// <summary>
	/// Gets or sets the airport name.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("name")]
	[JsonPropertyName("name")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the airport longitude in degrees.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("longitude_deg")]
	[JsonPropertyName("longitude_deg")]
	public double LongitudeDeg { get; set; } = 0.0;

	/// <summary>
	/// Gets or sets the airport latitude in degrees.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("latitude_deg")]
	[JsonPropertyName("latitude_deg")]
	public double LatitudeDeg { get; set; } = 0.0;

	/// <summary>
	/// Gets or sets the airport elevation in feet.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("elevation_ft")]
	[JsonPropertyName("elevation_ft")]
	public double? ElevationFt { get; set; }

	/// <summary>
	/// Gets or sets the continent where the airport is located.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("continent")]
	[JsonPropertyName("continent")]
	public string? Continent { get; set; }

	/// <summary>
	/// Gets or sets the ISO country code where the airport is located.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("iso_country")]
	[JsonPropertyName("iso_country")]
	public string? ISOCountry { get; set; }

	/// <summary>
	/// Gets or sets the ISO region code where the airport is located.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("iso_region")]
	[JsonPropertyName("iso_region")]
	public string? ISORegion { get; set; }

	/// <summary>
	/// Gets or sets the name of the municipality.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("municipality")]
	[JsonPropertyName("municipality")]
	public string? Municipality { get; set; }

	/// <summary>
	/// Gets or sets whether the airport has scheduled service.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("scheduled_service")]
	[JsonPropertyName("scheduled_service")]
	public string? ScheduledService { get; set; }

	/// <summary>
	/// Gets or sets the airport ICAO code.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("icao_code")]
	[JsonPropertyName("icao_code")]
	public string? ICAOCode { get; set; }

	/// <summary>
	/// Gets or sets the airport IATA code.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("iata_code")]
	[JsonPropertyName("iata_code")]
	public string? IATACode { get; set; }

	/// <summary>
	/// Gets or sets the GPS code associated with the location.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("gps_code")]
	[JsonPropertyName("gps_code")]
	public string? GPSCode { get; set; }

	/// <summary>
	/// Gets or sets the local code associated with the location.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("local_code")]
	[JsonPropertyName("local_code")]
	public string? LocalCode { get; set; }

	/// <summary>
	/// Gets or sets the home link for the airport.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("home_link")]
	[JsonPropertyName("home_link")]
	public string? HomeLink { get; set; }

	/// <summary>
	/// Gets or sets the Wikipedia link for the airport.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("wikipedia_link")]
	[JsonPropertyName("wikipedia_link")]
	public string? WikipediaLink { get; set; }

	/// <summary>
	/// Gets or sets the keywords associated with the airport.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("keywords")]
	[JsonPropertyName("keywords")]
	public string? Keywords { get; set; }
}
