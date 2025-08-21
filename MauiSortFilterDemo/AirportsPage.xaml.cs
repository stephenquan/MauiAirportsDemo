namespace MauiSortFilterDemo;

/// <summary>
/// Represents a page that displays airport-related content in the application.
/// </summary>
public partial class AirportsPage : ContentPage
{
	/// <summary>
	/// Gets the application's main view model.
	/// </summary>
	public AppViewModel VM { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="AirportsPage"/> class.
	/// </summary>
	public AirportsPage(AppViewModel vm)
	{
		this.VM = vm;
		this.BindingContext = this;

		InitializeComponent();

		this.Dispatcher.Dispatch(async () =>
		{
			this.IsBusy = true;
			VM.AirportSearchResults.Clear();
			await Task.Delay(100);
			await VM.PopulateAirports();
			await VM.ExecuteAirportSearchAsync();
			this.IsBusy = false;
		});

		this.VM.PropertyChanged += (s, e) =>
		{
			if (e.PropertyName == nameof(VM.AirportSearchText))
			{
				this.Dispatcher.Dispatch(async () =>
				{
					this.IsBusy = true;
					await VM.ExecuteAirportSearchAsync();
					this.IsBusy = false;
				});
			}
		};
	}
}
