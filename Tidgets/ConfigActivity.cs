using Android;
using Android.Appwidget;
using Android.Content;
using Android.Content.PM;
using Android.Views;

namespace Tidgets;

/// <summary>
/// Shown when a widget is added, to choose its source and set it up.
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
    /// Shows the setup form and wires up its controls.
    /// </summary>
    /// <param name="savedInstanceState">The previously saved instance state, if any.</param>
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetResult(Result.Canceled);
        WidgetId = Intent?.GetIntExtra(AppWidgetManager.ExtraAppwidgetId, AppWidgetManager.InvalidAppwidgetId) ?? AppWidgetManager.InvalidAppwidgetId;
        if (WidgetId == AppWidgetManager.InvalidAppwidgetId)
        {
            Finish();
            return;
        }
        SetContentView(Resource.Layout.activity_config);
        Spinner sourceSpinner = (Spinner)FindViewById(Resource.Id.source)!;
        EditText baseUrl = (EditText)FindViewById(Resource.Id.base_url)!;
        EditText apiKey = (EditText)FindViewById(Resource.Id.api_key)!;
        View apiKeyGroup = FindViewById(Resource.Id.api_key_group)!;
        Button grantLocation = (Button)FindViewById(Resource.Id.grant_location)!;
        ArrayAdapter<string> adapter = new(this, Android.Resource.Layout.SimpleSpinnerItem, WidgetSources.All.Select(x => x.Name).ToList());
        adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
        sourceSpinner.Adapter = adapter;

        // Reconfiguring a widget starts from its current setup.
        WidgetConfig? existing = new SettingsStore(this).GetConfig(WidgetId);
        int existingIndex = existing is null ? -1 : WidgetSources.All.ToList().FindIndex(x => x.Id == existing.SourceId);
        if (existingIndex >= 0)
        {
            sourceSpinner.SetSelection(existingIndex);
            baseUrl.Text = existing!.BaseUrl;
            apiKey.Text = existing.ApiKey;
        }
        bool firstSelection = true;
        sourceSpinner.ItemSelected += (sender, args) =>
        {
            WidgetSource source = WidgetSources.All[args.Position];
            if (!(firstSelection && existingIndex >= 0))
            {
                baseUrl.Text = source.DefaultBaseUrl;
            }
            firstSelection = false;
            apiKeyGroup.Visibility = source.UsesApiKey ? ViewStates.Visible : ViewStates.Gone;
            grantLocation.Visibility = source.UsesLocation ? ViewStates.Visible : ViewStates.Gone;
        };
        grantLocation.Click += (sender, args) => RequestPermissions([Manifest.Permission.AccessCoarseLocation], 1);
        ((Button)FindViewById(Resource.Id.add_widget)!).Click += (sender, args) =>
        {
            WidgetSource source = WidgetSources.All[sourceSpinner.SelectedItemPosition];
            new SettingsStore(this).SaveConfig(new WidgetConfig { WidgetId = WidgetId, SourceId = source.Id, BaseUrl = baseUrl.Text ?? string.Empty, ApiKey = apiKey.Text ?? string.Empty });
            WidgetProvider.Refresh(this);
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