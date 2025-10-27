using System.Diagnostics;

namespace MauiSortFilterDemo;

/// <summary>
/// Provides extension methods for the SQLiteConnection class.
/// </summary>
public static class SQLiteConnectionExtensions
{
	/// <summary>
	/// Asynchronously loads a database schema from a specified MAUI asset into the SQLite connection.
	/// </summary>
	/// <param name="db">The SQLite connection where the schema will be loaded.</param>
	/// <param name="assetName">The name of the MAUI asset containing the database schema.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	public static async Task LoadSchemaFromMauiAsset(this SQLite.SQLiteConnection db, string assetName)
	{
		using var stream = await FileSystem.OpenAppPackageFileAsync(assetName);
		using var reader = new StreamReader(stream);
		var schema = reader.ReadToEnd();
		await LoadSchema(db, schema);
	}

	/// <summary>
	/// Loads the specified database schema into the SQLite connection.
	/// </summary>
	/// <param name="db">The SQLite connection to which the schema will be loaded. Cannot be null.</param>
	/// <param name="schema">The schema definition to be loaded into the database. Cannot be null or empty.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	public static async Task LoadSchema(this SQLite.SQLiteConnection db, string schema)
	{
		await Task.Yield();
		db.RunInTransaction(() =>
		{
			string sql = string.Empty;
			foreach (var line in schema.Split(new[] { "\r\n", "\n", "\n" }, StringSplitOptions.RemoveEmptyEntries))
			{
				if (line.TrimStart().StartsWith("--"))
				{
					continue;
				}
				if (string.IsNullOrEmpty(line) || line.Length == 0)
				{
					continue;
				}
				sql += line.Trim() + " ";
				if (!char.IsWhiteSpace(line[0]) && line.TrimEnd().EndsWith(";"))
				{
					Trace.WriteLine("Executing SQL: " + sql);
					db.Execute(sql);
					sql = string.Empty;
				}
			}
		});
	}
}
