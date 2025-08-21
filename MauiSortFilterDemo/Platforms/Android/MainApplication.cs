using Android.App;
using Android.Runtime;

namespace MauiSortFilterDemo;

#pragma warning disable CS1591 // Suppress missing XML comment for publicly visible type or member

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
