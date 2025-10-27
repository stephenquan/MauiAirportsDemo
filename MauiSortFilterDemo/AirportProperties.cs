using System.Text.Json.Serialization;

namespace MauiSortFilterDemo;

/// <summary>
/// Represents secondary properties of an airport, including its identifier and geographic coordinates.
/// </summary>
public class AirportProperties
{
	/// <summary>
	/// Gets or sets the unique identifier for the airport.
	/// </summary>
	[JsonPropertyName("ident")]
	public string Ident { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the longitude of the airport in degrees.
	/// </summary>
	[JsonPropertyName("longitude")]
	public double Longitude { get; set; } = 0.0;

	/// <summary>
	/// Gets or sets the latitude of the airport in degrees.
	/// </summary>
	[JsonPropertyName("latitude")]
	public double Latitude { get; set; } = 0.0;
}
