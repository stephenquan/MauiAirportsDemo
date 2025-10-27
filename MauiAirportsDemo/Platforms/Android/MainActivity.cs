﻿// MainActivity.cs

using Android.App;
using Android.Content.PM;

namespace MauiAirportsDemo;

#pragma warning disable CS1591 // Suppress missing XML comment for publicly visible type or member

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
}
