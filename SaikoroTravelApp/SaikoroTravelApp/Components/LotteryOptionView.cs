using Android.Content;
using Android.Util;
using Android.Widget;

using SaikoroTravelApp.ViewModels;

using System;

namespace SaikoroTravelApp.Components
{
    internal class LotteryOptionView : LinearLayout
    {
        private readonly ImageView image;
        private readonly TextView title;
        private readonly TextView subTitle;

        private LotteryOptionViewModel viewModel;
        public LotteryOptionViewModel ViewModel
        {
            get => viewModel;
            set
            {
                if (viewModel != null)
                {
                    viewModel.PropertyChanged -= ViewModelPropertyChanged;
                }
                viewModel = value;
                viewModel.PropertyChanged += ViewModelPropertyChanged;
                Render();
            }
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Render();
        }

        public LotteryOptionView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            _ = Inflate(Context, Resource.Layout.component_lottery_option, this);

            image = FindViewById<ImageView>(Resource.Id.LotteryOptionSaikoroImage);
            title = FindViewById<TextView>(Resource.Id.LotteryOptionTitleText);
            subTitle = FindViewById<TextView>(Resource.Id.LotteryOptionSubTitleText);
        }

        private void Render()
        {
            var resId = viewModel.Number switch
            {
                1 => Resource.Drawable.saikoro1,
                2 => Resource.Drawable.saikoro2,
                3 => Resource.Drawable.saikoro3,
                4 => Resource.Drawable.saikoro4,
                5 => Resource.Drawable.saikoro5,
                6 => Resource.Drawable.saikoro6,
                _ => throw new NotSupportedException(),
            };
            image.SetImageResource(resId);
            title.Text = viewModel.Title;
            subTitle.Text = viewModel.SubTitle;
        }
    }
}