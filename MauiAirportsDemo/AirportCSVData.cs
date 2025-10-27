// AirportCSVData.cs

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
	public int Id { get; set; } = 0;

	/// <summary>
	/// Gets or sets the airport identifier.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("ident")]
	public string Ident { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the airport type.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("type")]
	public string Type { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the airport name.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("name")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the airport longitude in degrees.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("longitude_deg")]
	public double Longitude { get; set; } = 0.0;

	/// <summary>
	/// Gets or sets the airport latitude in degrees.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("latitude_deg")]
	public double Latitude { get; set; } = 0.0;

	/// <summary>
	/// Gets or sets the airport IATA code.
	/// </summary>
	[CsvHelper.Configuration.Attributes.Name("iata_code")]
	public string IATA { get; set; } = string.Empty;

}
