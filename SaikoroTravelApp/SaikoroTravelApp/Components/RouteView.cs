using Android.Content;
using Android.Util;
using Android.Widget;

using SaikoroTravelApp.ViewModels;

namespace SaikoroTravelApp.Components
{
    /// <summary>
    /// 行程を表示するUIコンポーネントを表現します。
    /// </summary>
    internal class RouteView : FixedLinearLayout
    {
        private readonly TextView titleText;
        private readonly TextView departureName;
        private readonly TextView destinationName;
        private readonly TextView departureTime;
        private readonly TextView destinationTime;
        private readonly TextView trainName;
        private readonly TextView trainDetail;
        private readonly TextView price;
        private readonly TextView message1;
        private readonly TextView message2;
        private readonly TextView message3;

        /// <summary>
        /// <see cref="RouteView"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attributeSet"></param>
        public RouteView(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
        {
            _ = Inflate(context, Resource.Layout.component_route, this);

            titleText = FindViewById<TextView>(Resource.Id.RouteTitleText);
            departureName = FindViewById<TextView>(Resource.Id.RouteDepartureName);
            destinationName = FindViewById<TextView>(Resource.Id.RouteDestinationName);
            departureTime = FindViewById<TextView>(Resource.Id.RouteDepartureTime);
            destinationTime = FindViewById<TextView>(Resource.Id.RouteDestinationTime);
            trainName = FindViewById<TextView>(Resource.Id.RouteTrainName);
            trainDetail = FindViewById<TextView>(Resource.Id.RouteTrainDetails);
            price = FindViewById<TextView>(Resource.Id.RoutePriceText);
            message1 = FindViewById<TextView>(Resource.Id.RouteMessageLine1);
            message2 = FindViewById<TextView>(Resource.Id.RouteMessageLine2);
            message3 = FindViewById<TextView>(Resource.Id.RouteMessageLine3);
        }

        private RouteViewModel viewModel;

        /// <summary>
        /// このコンポーネントに関連付けられたビューモデルを取得または設定します。
        /// </summary>
        public RouteViewModel ViewModel
        {
            get => viewModel;
            set
            {
                if (ReferenceEquals(ViewModel, value))
                {
                    return;
                }

                if (!(viewModel is null))
                {
                    viewModel.PropertyChanged -= ViewModelPropertyChanged;
                }
                value.PropertyChanged += ViewModelPropertyChanged;

                if (viewModel != value)
                {
                    viewModel = value;
                    Render();
                }
                else
                {
                    viewModel = value;
                }
            }
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // 更新された要素のみ再描画するほうがなお良い
            Render();
        }

        private void Render()
        {
            titleText.Text = viewModel.TitleText;
            departureName.Text = viewModel.DepartureName;
            destinationName.Text = viewModel.DestinationName;
            departureTime.Text = viewModel.DepartureTime;
            destinationTime.Text = viewModel.DestinationTime;
            trainName.Text = viewModel.TrainName;
            trainDetail.Text = viewModel.TrainDetail;
            price.Text = viewModel.Price;
            message1.Text = viewModel.Message1;
            message2.Text = viewModel.Message2;
            message3.Text = viewModel.Message3;
        }

        protected override int UnitWidth => 8500;

        protected override int UnitHeight => 5750;
    }
}