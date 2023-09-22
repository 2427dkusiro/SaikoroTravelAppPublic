
using AndroidX.Fragment.App;

using Java.Lang;

using SaikoroTravelApp.Fragments.MainTab;

using System;

using FragmentManager = AndroidX.Fragment.App.FragmentManager;

namespace SaikoroTravelApp
{
    internal class MainViewPageAdapter : FragmentPagerAdapter
    {
        public MainViewPageAdapter(FragmentManager fragmentManager) : base(fragmentManager)
        {

        }

        public override int Count => 3;

        public override Fragment GetItem(int position)
        {
            return position switch
            {
                0 => new Home(),
                1 => new Timeline(),
                2 => new Other(),
                _ => throw new NotSupportedException(),
            };
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return new Java.Lang.String(position switch
            {
                0 => nameof(Home),
                1 => nameof(Timeline),
                2 => nameof(Other),
                _ => throw new NotSupportedException(),
            });
        }
    }
}