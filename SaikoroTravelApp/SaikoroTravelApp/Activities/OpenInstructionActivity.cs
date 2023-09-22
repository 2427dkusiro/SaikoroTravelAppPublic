using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;

using AndroidX.AppCompat.App;

using SaikoroTravelApp.Backends;
using SaikoroTravelApp.Components;
using SaikoroTravelApp.ViewModels;

using SaikoroTravelCommon.Models;

using System;

namespace SaikoroTravelApp.Activities
{
    [Activity(Label = "OpenInstructionActivity", Theme = "@style/AppTheme.NoActionBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    public class OpenInstructionActivity : AppCompatActivity
    {
        private Logger logger;
        private DependencyProvider dependencyProvider;
        private RouteManager routeManager;
        private Instruction instruction;

        private InstructionView instructionView;
        private InstructionViewModel viewModel;

        private TextView label;
        private Button button;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            SetContentView(Resource.Layout.activity_openInstruction);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                Window.DecorView.WindowInsetsController.Hide(WindowInsets.Type.StatusBars());
            }

            dependencyProvider = DependencyProvider.Default;
            logger = Logger.GetInstance(this, dependencyProvider.RequestTimer(this), dependencyProvider.RequestConfigManager(this).User);
            routeManager = dependencyProvider.RequestRouteManager(this);

            InstructionManager instructionManager = dependencyProvider.RequestInstructionManager(this);
            var id = Intent.GetStringExtra("id");

            try
            {
                instruction = instructionManager.FindById(id);
            }
            catch (Exception ex)
            {
                logger.Error($"指示アクティビティの引数の指示ID'{id}'が見つかりません", ex);
                throw;
            }
            logger.Info($"指示'{id}'を開く指示開封アクティビティが開始されました");

            instructionView = FindViewById<InstructionView>(Resource.Id.InstructionView);
            label = FindViewById<TextView>(Resource.Id.OpenInstructionLabel);
            button = FindViewById<Button>(Resource.Id.CloseButton);
            button.Click += ButtonClick;

            viewModel = new InstructionViewModel()
            {
                Content = instruction.Content,
                IsOpend = false
            };
            viewModel.AnimationStarted += AnimationStarted;
            viewModel.InstructionOpened += InstructionOpened;
            instructionView.ViewModel = viewModel;
        }

        private void AnimationStarted(object sender, EventArgs e)
        {
            label.Visibility = Android.Views.ViewStates.Gone;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private async void InstructionOpened(object sender, EventArgs e)
        {
            await logger.ReportInstructionOpened(instruction);

            foreach (RouteUpdateAction update in instruction.RouteUpdates)
            {
                update.Apply(routeManager);
            }

            instruction.SetState(RouteState.Executed);
            button.Visibility = Android.Views.ViewStates.Visible;
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            Finish();
        }
    }
}