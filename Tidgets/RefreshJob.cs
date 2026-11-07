using Android.App;
using Android.App.Job;
using Android.Content;
using Android.Util;
using JavaClass = Java.Lang.Class;

namespace Tidgets;

/// <summary>
/// Background job that refreshes every widget once the phone has a network connection, so a refresh missed while offline or asleep runs as soon as the connection is back.
/// </summary>
[Service(Permission = "android.permission.BIND_JOB_SERVICE", Exported = true)]
public class RefreshJob : JobService
{
    /// <summary>
    /// Asks for a refresh of every widget, which runs straight away if there's a connection, or as soon as there is one.
    /// </summary>
    /// <param name="context">The context.</param>
    public static void Schedule(Context context)
    {
        JobScheduler? scheduler = (JobScheduler?)context.GetSystemService(Context.JobSchedulerService);
        int jobId = context.Resources!.GetInteger(Resource.Integer.refresh_job_id);
        JobInfo? job = new JobInfo.Builder(jobId, new ComponentName(context, JavaClass.FromType(typeof(RefreshJob)))).SetRequiredNetworkType(NetworkType.Any)?.Build();
        if (scheduler is not null && job is not null)
        {
            scheduler.Schedule(job);
        }
    }

    /// <summary>
    /// Refreshes every placed widget in the background, asking Android to try again later if any couldn't reach its site.
    /// </summary>
    /// <param name="parameters">The job's parameters.</param>
    public override bool OnStartJob(JobParameters? parameters)
    {
        if (parameters is null)
        {
            return false;
        }
        Task.Run(async () =>
        {
            bool failed = false;
            try
            {
                foreach (WidgetSource source in WidgetSources.All)
                {
                    failed |= await source.CreateWidget().UpdateAllAsync(this);
                }
            }
            catch (Exception exception)
            {
                Log.Warn("Tidgets", $"Refresh job failed: {exception}");
                failed = true;
            }
            finally
            {
                JobFinished(parameters, failed);
            }
        });
        return true;
    }

    /// <summary>
    /// Asks Android to run the job again if it was stopped part way, for example because the connection dropped.
    /// </summary>
    /// <param name="parameters">The job's parameters.</param>
    public override bool OnStopJob(JobParameters? parameters)
    {
        return true;
    }
}