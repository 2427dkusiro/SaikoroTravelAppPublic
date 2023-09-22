using Android.App;
using Android.App.Job;
using Android.Content;

using System.Threading.Tasks;

namespace SaikoroTravelApp
{
    [Service(Name = "com.kujiro.saikorotravelapp.SyncLogJob",
         Permission = "android.permission.BIND_JOB_SERVICE")]
    public class SyncLogJob : JobService
    {
        private LocationService locationService;

        public override bool OnStartJob(JobParameters jobParams)
        {
            _ = Task.Run(async () =>
            {
                locationService = new LocationService(this);
                await locationService.GetCurrentLocation();
                JobFinished(jobParams, false);
            });

            return true;
        }

        public override bool OnStopJob(JobParameters jobParams)
        {
            locationService?.Dispose();
            return false;
        }
    }

    public static class JobSchedulerHelpers
    {
        public static JobInfo.Builder CreateJobBuilderUsingJobId<T>(this Context context, int jobId) where T : JobService
        {
            var javaClass = Java.Lang.Class.FromType(typeof(T));
            var componentName = new ComponentName(context, javaClass);
            return new JobInfo.Builder(jobId, componentName);
        }
    }
}