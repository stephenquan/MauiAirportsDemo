// MainPage.xaml.cs

namespace MauiAirportsDemo;

/// <summary>
/// Represents the main page of the application.
/// </summary>
/// <remarks>This page serves as the entry point for the application's user interface.
/// It includes navigation functionality to other pages within the application.</remarks>
public partial class MainPage : ContentPage
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MainPage"/> class.
	/// </summary>
	public MainPage()
	{
		InitializeComponent();
	}

	async void OnNavigateClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync(((Button)sender).CommandParameter.ToString());
	}
}
