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
	[SQLite.PrimaryKey]
	public string Name { get; set; } = string.Empty;
}
