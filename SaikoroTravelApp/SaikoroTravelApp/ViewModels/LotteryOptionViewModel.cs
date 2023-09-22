namespace SaikoroTravelApp.ViewModels
{
    internal class LotteryOptionViewModel : ViewModelBase
    {
        private int number;
        public int Number
        {
            get => number;
            set => SetProperty(ref number, value);
        }

        private string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string subTitle;
        public string SubTitle
        {
            get => subTitle;
            set => SetProperty(ref subTitle, value);
        }
    }
}