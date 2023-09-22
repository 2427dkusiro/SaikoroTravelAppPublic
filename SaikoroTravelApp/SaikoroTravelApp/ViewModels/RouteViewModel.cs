using System;

namespace SaikoroTravelApp.ViewModels
{
    /// <summary>
    /// 行程ビューのビューモデルを表現します。
    /// </summary>
    internal class RouteViewModel : ViewModelBase
    {
        private string titleText;

        /// <summary>
        /// タイトルテキストを取得します。
        /// </summary>
        public string TitleText
        {
            get => titleText;
            set => SetProperty(ref titleText, value);
        }

        private string departureName;

        /// <summary>
        /// 出発地点名を取得または設定します。
        /// </summary>
        public string DepartureName
        {
            get => departureName;
            set => SetProperty(ref departureName, value);
        }

        private string destinationName;

        /// <summary>
        /// 到着地点名を取得または設定します。
        /// </summary>
        public string DestinationName
        {
            get => destinationName;
            set => SetProperty(ref destinationName, value);
        }

        private string departureTime;

        /// <summary>
        /// 出発時刻を取得または設定します。
        /// </summary>
        public string DepartureTime
        {
            get => departureTime;
            set => SetProperty(ref departureTime, value);
        }

        private string destinationTime;

        /// <summary>
        /// 到着時刻を取得または設定します。
        /// </summary>
        public string DestinationTime
        {
            get => destinationTime;
            set => SetProperty(ref destinationTime, value);
        }

        private string trainName;

        /// <summary>
        /// 移動手段名を取得または設定します。
        /// </summary>
        public string TrainName
        {
            get => trainName;
            set => SetProperty(ref trainName, value);
        }

        private string trainDetail;

        /// <summary>
        /// 移動手段詳細情報を取得または設定します。
        /// </summary>
        public string TrainDetail
        {
            get => trainDetail;
            set => SetProperty(ref trainDetail, value);
        }

        private string price;

        /// <summary>
        /// 金額を表す文字列を取得または設定します。
        /// </summary>
        public string Price
        {
            get => price;
            set => SetProperty(ref price, value);
        }

        private string message1;

        /// <summary>
        /// 1行目に表示するメッセージを取得または設定します。
        /// </summary>
        public string Message1
        {
            get => message1;
            set => SetProperty(ref message1, value);
        }

        private string message2;

        /// <summary>
        /// 2行目に表示するメッセージを取得または設定します。
        /// </summary>
        public string Message2
        {
            get => message2;
            set => SetProperty(ref message2, value);
        }

        private string message3;

        /// <summary>
        /// 3行目に表示するメッセージを取得または設定します。
        /// </summary>
        public string Message3
        {
            get => message3;
            set => SetProperty(ref message3, value);
        }

        public override bool Equals(object obj)
        {
            return obj is RouteViewModel model &&
                   titleText == model.titleText &&
                   departureName == model.departureName &&
                   destinationName == model.destinationName &&
                   departureTime == model.departureTime &&
                   destinationTime == model.destinationTime &&
                   trainName == model.trainName &&
                   trainDetail == model.trainDetail &&
                   price == model.price &&
                   message1 == model.message1 &&
                   message2 == model.message2 &&
                   message3 == model.message3;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(titleText);
            hash.Add(departureName);
            hash.Add(destinationName);
            hash.Add(departureTime);
            hash.Add(destinationTime);
            hash.Add(trainName);
            hash.Add(trainDetail);
            hash.Add(price);
            hash.Add(message1);
            hash.Add(message2);
            hash.Add(message3);
            return hash.ToHashCode();
        }

        public static bool operator ==(RouteViewModel left, RouteViewModel right)
        {
            return left is null ? right is null : left.Equals(right);
        }

        public static bool operator !=(RouteViewModel left, RouteViewModel right)
        {
            return !(left == right);
        }
    }
}