using Android;
using Android.Content;
using Android.Content.PM;
using Android.Locations;

namespace Tidgets;

/// <summary>
/// Finds the device's current location without keeping the GPS on.
/// </summary>
/// <param name="context">The context used to reach the location manager.</param>
/// <param name="settings">The settings store, for remembering the location.</param>
public class LocationService(Context context, SettingsStore settings)
{
    /// <summary>
    /// Gets the most recent location the device knows, falling back to the remembered one.
    /// </summary>
    public (double Latitude, double Longitude)? GetLocation()
    {
        Location? best = null;
        if (context.CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted && context.GetSystemService(Context.LocationService) is LocationManager manager)
        {
            string[] providers = [LocationManager.NetworkProvider!, LocationManager.PassiveProvider!];
            foreach (string provider in providers)
            {
                try
                {
                    Location? location = manager.GetLastKnownLocation(provider);
                    if (location is not null && (best is null || location.Time > best.Time))
                    {
                        best = location;
                    }
                }
                catch (Java.Lang.SecurityException)
                {
                    // Background location isn't granted, so fall back to the remembered location.
                }
            }
        }
        if (best is null)
        {
            return settings.GetSavedLocation();
        }
        settings.SaveLocation(best.Latitude, best.Longitude);
        return (best.Latitude, best.Longitude);
    }
}