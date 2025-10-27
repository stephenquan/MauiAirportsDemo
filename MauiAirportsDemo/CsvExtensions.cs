// CsvExtensions.cs

using CsvHelper;

namespace MauiAirportsDemo;

/// <summary>
/// Provides extension methods for handling CSV data within a MAUI application.
/// </summary>
public static class CsvExtensions
{
	/// <summary>
	/// Asynchronously loads and parses a CSV file from a specified MAUI asset into
	/// a collection of records of type <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T">The type of records to be returned, which must match the structure of the CSV data.</typeparam>
	/// <param name="mauiAsset">The name of the MAUI asset file containing the CSV data. This parameter cannot be null or empty.</param>
	/// <returns>A task representing the asynchronous operation.
	/// The task result contains an <see cref="IEnumerable{T}"/> of records parsed from the CSV file.</returns>
	public static async Task<List<T>> LoadCSVFromMauiAsset<T>(string mauiAsset)
	{
		using (var stream = await FileSystem.OpenAppPackageFileAsync(mauiAsset))
		using (var reader = new StreamReader(stream))
		using (var csv = new CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
		{
			return csv.GetRecords<T>().ToList();
		}
	}
}