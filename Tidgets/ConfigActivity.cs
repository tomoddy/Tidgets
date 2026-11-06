using Android;
using Android.Appwidget;
using Android.Content;
using Android.Content.PM;
using Android.Views;

namespace Tidgets;

/// <summary>
/// Shown when a widget is added or reconfigured, to set up the site it shows.
/// </summary>
[Activity(Name = "uk.co.tzer0m.tidgets.ConfigActivity", Label = "@string/app_name", Exported = true)]
[IntentFilter([AppWidgetManager.ActionAppwidgetConfigure])]
public class ConfigActivity : Activity
{
    /// <summary>
    /// The Android ID of the widget being set up.
    /// </summary>
    private int WidgetId = AppWidgetManager.InvalidAppwidgetId;

    /// <summary>
    /// Shows the setup form for the widget's source and wires up its controls.
    /// </summary>
    /// <param name="savedInstanceState">The previously saved instance state, if any.</param>
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetResult(Result.Canceled);
        WidgetId = Intent?.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, AppWidgetManager.InvalidAppwidgetId) ?? AppWidgetManager.InvalidAppwidgetId;
        string? provider = WidgetId == AppWidgetManager.InvalidAppwidgetId ? null : AppWidgetManager.GetInstance(this)?.GetAppWidgetInfo(WidgetId)?.Provider?.ClassName;
        WidgetSource? source = provider is null ? null : WidgetSources.FindByProvider(provider);
        if (source is null)
        {
            Finish();
            return;
        }
        SetContentView(Resource.Layout.activity_config);
        ((TextView)FindViewById(Resource.Id.source_name)!).Text = source.Name;
        EditText baseUrl = (EditText)FindViewById(Resource.Id.base_url)!;
        EditText apiKey = (EditText)FindViewById(Resource.Id.api_key)!;
        FindViewById(Resource.Id.api_key_group)!.Visibility = source.UsesApiKey ? ViewStates.Visible : ViewStates.Gone;
        Button grantLocation = (Button)FindViewById(Resource.Id.grant_location)!;
        grantLocation.Visibility = source.UsesLocation ? ViewStates.Visible : ViewStates.Gone;

        // Reconfiguring a widget starts from its current setup.
        WidgetConfig? existing = new SettingsStore(this).GetConfig(WidgetId);
        baseUrl.Text = existing?.BaseUrl ?? source.DefaultBaseUrl;
        apiKey.Text = existing?.ApiKey ?? string.Empty;
        grantLocation.Click += (sender, args) => RequestPermissions([Manifest.Permission.AccessCoarseLocation], 1);
        ((Button)FindViewById(Resource.Id.save)!).Click += (sender, args) =>
        {
            new SettingsStore(this).SaveConfig(new WidgetConfig { WidgetId = WidgetId, SourceId = source.Id, BaseUrl = baseUrl.Text ?? string.Empty, ApiKey = apiKey.Text ?? string.Empty });
            SourceWidget.Refresh(this, source, WidgetId);
            Intent result = new();
            result.PutExtra(AppWidgetManager.ExtraAppwidgetId, WidgetId);
            SetResult(Result.Ok, result);
            Finish();
        };
    }

    /// <summary>
    /// Asks for background location once foreground location is granted.
    /// </summary>
    /// <param name="requestCode">The request code.</param>
    /// <param name="permissions">The permissions that were requested.</param>
    /// <param name="grantResults">Whether each permission was granted.</param>
    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        if (requestCode == 1 && grantResults.Length > 0 && grantResults[0] == Permission.Granted && OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            RequestPermissions([Manifest.Permission.AccessBackgroundLocation], 2);
        }
    }
}