﻿using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Java.Net;

namespace Boilerplate.Client.Maui.Platforms.Android;

[IntentFilter([Intent.ActionView],
                        DataScheme = "https",
                        DataHosts = ["todo.bitplatform.dev", "adminpanel.bitplatform.dev"],
                        DataPathPrefix = "/",
                        AutoVerify = true,
                        Categories = [Intent.ActionView, Intent.CategoryDefault, Intent.CategoryBrowsable])]

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleInstance,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        var url = Intent?.DataString;
        if (url is not null && App.Current!.MainPage is NavigationPage navPage && navPage.CurrentPage is MainPage mainPage)
        {
            mainPage.BlazorWebView.StartPath = new URL(url).File ?? "/";
        }
    }

    protected override void OnNewIntent(Intent? intent)
    {
        base.OnNewIntent(intent);

        var action = intent!.Action;
        var url = intent.DataString;
        if (action is Intent.ActionView && string.IsNullOrWhiteSpace(url) is false)
        {
            var pubSubService = IPlatformApplication.Current!.Services.GetRequiredService<IPubSubService>();
            pubSubService.Publish(PubSubMessages.NAVIGATE, new URL(url).File ?? "/");
        }
    }
}
