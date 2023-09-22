using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Views;

using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using AndroidX.ViewPager.Widget;

using FCMClient;

using Google.Android.Material.Tabs;

using SaikoroTravelApp.Activities;
using SaikoroTravelApp.Backends;

using System.Threading.Tasks;

using AlertDialog = AndroidX.AppCompat.App.AlertDialog;

namespace SaikoroTravelApp
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		private Logger logger;
		private DependencyProvider dependencyProvider;

		protected override async void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			Xamarin.Essentials.Platform.Init(this, savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			/* デバッグ用 */
			/*
            new SharedPrefWrapper(this).DeleteTable("Route_State");
            new SharedPrefWrapper(this).DeleteTable("Instruction_State");
            new SharedPrefWrapper(this).DeleteTable("Lottery_State");
            new SharedPrefWrapper(this).DeleteTable("Log");
            */

			// var token = await MyFirebaseIIDService.GetToken();

			dependencyProvider = DependencyProvider.Default;
			ConfigManager configManager = dependencyProvider.RequestConfigManager(this);

			Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);

            if (!configManager.TryGetUser(out _))
            {
                _ = new AlertDialog.Builder(this)
                    .SetTitle("初期設定")
                    .SetMessage("設定情報が登録されていません。初期設定を行います。")
                    .SetPositiveButton("OK", (sender, e) =>
                    {
                        var intent = new Intent(this, typeof(RegisterUserActivity));
                        StartActivityForResult(intent, 1);
                    })
                    .Show();
                return;
            }
			_ = Init();
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		protected override async void OnActivityResult(int requestCode, [Android.Runtime.GeneratedEnum] Result resultCode, Intent data)
		{
			if (requestCode == 1)
			{
				if (resultCode == Result.Ok)
				{
					_ = Init();
				}
			}
			base.OnActivityResult(requestCode, resultCode, data);
		}

		private LocationService locationService;

		private async Task Init()
		{
			logger = Logger.GetInstance(this, dependencyProvider.RequestTimer(this), dependencyProvider.RequestConfigManager(this).User);
			logger.Info("Mainアクティビティが開始されました");

			ViewPager pager = FindViewById<ViewPager>(Resource.Id.mainPager);
			pager.Adapter = new MainViewPageAdapter(SupportFragmentManager);

			TabLayout tabLayout = FindViewById<TabLayout>(Resource.Id.mainTab);
			tabLayout.SetupWithViewPager(pager);

			locationService = new LocationService(this);
			await locationService.GetCurrentLocation();
			// await MyFirebaseIIDService.UpdateToken(this);

			BuildJob();
		}

		private void BuildJob()
		{
			JobInfo.Builder jobBuilder = this.CreateJobBuilderUsingJobId<SyncLogJob>(1);
			JobInfo jobInfo = jobBuilder
				.SetPeriodic(15 * 60 * 1000L)
				.SetRequiredNetworkType(NetworkType.None)
				.Build();
			var jobScheduler = (JobScheduler)GetSystemService(JobSchedulerService);
			_ = jobScheduler.Schedule(jobInfo);
		}

		public override bool OnCreateOptionsMenu(IMenu menu)
		{
			MenuInflater.Inflate(Resource.Menu.menu_main, menu);
			return true;
		}

		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			var id = item.ItemId;
			return id == Resource.Id.action_settings || base.OnOptionsItemSelected(item);
		}
	}
}
