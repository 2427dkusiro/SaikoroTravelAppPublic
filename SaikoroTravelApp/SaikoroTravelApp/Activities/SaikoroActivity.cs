using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.AppCompat.App;

using SaikoroTravelApp.Backends;

using System;

namespace SaikoroTravelApp.Activities
{
    [Activity(Label = "SaikoroActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class SaikoroActivity : AppCompatActivity
    {
        private Logger logger;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_saikoro);
            DependencyProvider dependencyProvider = DependencyProvider.Default;
            logger = Logger.GetInstance(this, dependencyProvider.RequestTimer(this), dependencyProvider.RequestConfigManager(this).User);

            logger.Info("サイコロ画面開始");
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                Window.DecorView.WindowInsetsController.Hide(WindowInsets.Type.StatusBars());
            }

            var number = Intent.GetIntExtra("number", -1);
            if (number == -1)
            {
                throw new NotSupportedException();
            }

            VideoView videoView = FindViewById<VideoView>(Resource.Id.SaikoroVideoView);
            var path = "android.resource://" + PackageName + "/" + GetResourceId(number);
            var uri = Android.Net.Uri.Parse(path);

            videoView.SetAudioFocusRequest(Android.Media.AudioFocus.None);
            videoView.SetVideoURI(uri);
            videoView.SeekTo(1);

            Button playButton = FindViewById<Button>(Resource.Id.PlayVideoButton);
            playButton.Click += (sender, e) =>
            {
                playButton.Visibility = ViewStates.Gone;
                videoView.Start();
            };

            Button finishButton = FindViewById<Button>(Resource.Id.FinishVideoButton);
            finishButton.Click += (sender, e) =>
            {
                SetResult(Result.Ok);
                Finish();
            };

            videoView.Completion += (sender, e) =>
            {
                logger.Info("サイコロ動画終了");
                finishButton.Visibility = ViewStates.Visible;
            };
        }

        private int GetResourceId(int num)
        {
            return num switch
            {
                1 => Resource.Raw.d1,
                2 => Resource.Raw.d2,
                3 => Resource.Raw.d3,
                4 => Resource.Raw.d4,
                5 => Resource.Raw.d5,
                6 => Resource.Raw.d6,
                _ => throw new NotSupportedException()
            };
        }
    }
}