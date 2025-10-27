// AirportSQLData.cs

using System.Text.Json;

namespace MauiAirportsDemo;

/// <summary>
/// Represents the data model for an airport in the SQLite database.
/// </summary>
[SQLite.Table("Airports")]
public class AirportSQLData
{
	/// <summary>
	/// Gets or sets the name associated with the object.
	/// </summary>
	[SQLite.Column("Id")]
	public int Id { get; set; } = 0;

	/// <summary>
	/// Gets or sets the name associated with the object.
	/// </summary>
	[SQLite.Column("Name")]
	public string Name { get; set; } = string.Empty;

	/// <summary>
	/// Gets or sets the properties associated with the object in JSON format.
	/// </summary>
	[SQLite.Column("Properties")]
	public string PropertiesText { get; set; } = string.Empty;

	/// <summary>
	/// Gets the deserialized properties of the airport.
	/// </summary>
	[SQLite.Ignore]
	public AirportProperties? Properties
	{
		get => JsonSerializer.Deserialize<AirportProperties>(PropertiesText);
		set => PropertiesText = JsonSerializer.Serialize(value);
	}
}