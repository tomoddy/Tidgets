namespace Tidgets;

/// <summary>
/// Explains how to add a widget, since all setup happens when one is added.
/// </summary>
[Activity(Label = "@string/app_name", MainLauncher = true)]
public class MainActivity : Activity
{
    /// <summary>
    /// Shows the instructions.
    /// </summary>
    /// <param name="savedInstanceState">The previously saved instance state, if any.</param>
    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);
    }
}