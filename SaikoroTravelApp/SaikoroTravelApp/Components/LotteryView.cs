using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

using SaikoroTravelCommon.Models;

namespace SaikoroTravelApp.Components
{
    internal class LotteryView : LinearLayout
    {
        private readonly LinearLayout layout;

        public LotteryView(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            View view = Inflate(Context, Resource.Layout.component_lottery_view, this);
            layout = view.FindViewById<LinearLayout>(Resource.Id.LotteryView);
        }

        private Lottery lottery;

        public Lottery Lottery
        {
            get => lottery;
            set
            {
                lottery = value;
                Render();
            }
        }

        private void Render()
        {
            layout.RemoveAllViews();

            var layoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 0)
            {
                Weight = 1
            };
            var lineLayoutParams = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, 6);
            lineLayoutParams.SetMargins(8, 6, 8, 6);
            for (var i = 0; i < lottery.LotteryOptions.Count; i++)
            {
                LotteryOption option = lottery.LotteryOptions[i];
                var view = new LotteryOptionView(Context, null)
                {
                    LayoutParameters = layoutParams,
                    ViewModel = new ViewModels.LotteryOptionViewModel()
                    {
                        Number = option.Number,
                        Title = option.Title,
                        SubTitle = option.SubTitle
                    }
                };
                layout.AddView(view);

                if (i != lottery.LotteryOptions.Count - 1)
                {
                    var line = new LinearLayout(Context)
                    {
                        LayoutParameters = lineLayoutParams
                    };
                    line.SetBackgroundColor(Color.Rgb(0xcc, 0xcc, 0xcc));
                    layout.AddView(line);
                }
            }
        }
    }
}