namespace MauiSortFilterDemo;

/// <summary>
/// Represents an airport with a name.
/// </summary>
[SQLite.Table("Airport")]
public partial class Airport
{
	/// <summary>
	/// Gets or sets the name associated with the object.
	/// </summary>
	[SQLite.Column("Name")]
	//[SQLite.PrimaryKey]
	[SQLite.Indexed]
	[CsvHelper.Configuration.Attributes.Name("name")]
	public string Name { get; set; } = string.Empty;
}
