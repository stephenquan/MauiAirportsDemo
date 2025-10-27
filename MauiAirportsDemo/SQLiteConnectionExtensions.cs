// SQLiteConnectionExtensions.cs

using System.Diagnostics;
using System.Text.Json;
using SQLite;
using SQLitePCL;

namespace MauiAirportsDemo;

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
	public static async Task LoadSchemaFromMauiAsset(this SQLiteConnection db, string assetName)
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

	/// <summary>
	/// Registers JSON-related functions with the specified SQLite database connection.
	/// </summary>
	/// <param name="db">The SQLite connection to which the JSON functions will be added. Cannot be null.</param>
	public static void CreateJsonFunctions(this SQLiteConnection db)
	{
		SQLitePCL.raw.sqlite3_create_function(
			db.Handle,
			"JsonProperty",
			2,
			SQLitePCL.raw.SQLITE_UTF8 | SQLitePCL.raw.SQLITE_DETERMINISTIC,
			null,
			JsonProperty);
	}

	static void JsonProperty(sqlite3_context ctx, object user_data, sqlite3_value[] args)
	{
		try
		{
			var json = raw.sqlite3_value_text(args[0]).utf8_to_string();
			var propertyName = raw.sqlite3_value_text(args[1]).utf8_to_string();
			if (string.IsNullOrEmpty(json))
			{
				SetResult(ctx, null);
				return;
			}
			if (string.IsNullOrEmpty(propertyName))
			{
				SetResult(ctx, null);
				return;
			}
			var doc = JsonDocument.Parse(json);
			if (doc.RootElement.TryGetProperty(propertyName, out var property))
			{
				SetResult(ctx, property);
				return;
			}
			SetResult(ctx, null);
		}
		catch (Exception)
		{
		}
		SetResult(ctx, 0);
	}
	/// <summary>
	/// Sets the result of a SQLite function call based on the type of the provided result object.
	/// </summary>
	/// <param name="ctx">The SQLite function context.</param>
	/// <param name="result">The result object to be returned to SQLite. Supported types are <c>null</c>, <see cref="string"/>, <see cref="double"/>, and <see cref="int"/>.</param>
	static void SetResult(sqlite3_context ctx, object? result)
	{
		switch (result)
		{
			case null:
				raw.sqlite3_result_null(ctx);
				break;
			case string s:
				raw.sqlite3_result_text(ctx, utf8z.FromString(s));
				break;
			case double d:
				raw.sqlite3_result_double(ctx, d);
				break;
			case float f:
				raw.sqlite3_result_double(ctx, f);
				break;
			case int i:
				raw.sqlite3_result_int(ctx, i);
				break;
			case long l:
				raw.sqlite3_result_int64(ctx, l);
				break;
			case bool b:
				raw.sqlite3_result_int(ctx, b ? 1 : 0);
				break;
			case JsonElement je:
				switch (je.ValueKind)
				{
					case JsonValueKind.String:
						raw.sqlite3_result_text(ctx, utf8z.FromString(je.GetString() ?? string.Empty));
						break;
					case JsonValueKind.Number:
						if (je.TryGetInt64(out long longValue))
						{
							raw.sqlite3_result_int64(ctx, longValue);
						}
						else if (je.TryGetDouble(out double doubleValue))
						{
							raw.sqlite3_result_double(ctx, doubleValue);
						}
						else
						{
							raw.sqlite3_result_null(ctx);
						}
						break;
					case JsonValueKind.True:
						raw.sqlite3_result_int(ctx, 1);
						break;
					case JsonValueKind.False:
						raw.sqlite3_result_int(ctx, 0);
						break;
					default:
						raw.sqlite3_result_null(ctx);
						break;
				}
				break;
			default:
				if (result?.ToString() is string str && !string.IsNullOrEmpty(str))
				{
					raw.sqlite3_result_text(ctx, utf8z.FromString(str));
				}
				else
				{
					raw.sqlite3_result_null(ctx);
				}
				break;
		}
	}

	/// <summary>
	/// Sets an error result for a SQLite function call.
	/// This helper method is used within custom SQLite function delegates to report errors back to SQLite.
	/// It sets the error message and error code in the SQLite context, ensuring that the calling SQL statement
	/// receives an appropriate error response.
	/// </summary>
	/// <param name="ctx">The SQLite function context to set the error result for.</param>
	/// <param name="ex">The exception containing the error details to report.</param>
	static void SetResultError(sqlite3_context ctx, Exception ex)
	{
		raw.sqlite3_result_error(ctx, utf8z.FromString(ex.Message));
	}
}
