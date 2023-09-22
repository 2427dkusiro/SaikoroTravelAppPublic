using SaikoroTravelCommon.IO;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace SaikoroTravelCommon.Models
{
    /// <summary>
    /// 行程を表現します。
    /// </summary>
    public class Route : INotifyPropertyChanged
    {
        public Route(string id, string displayName, int day, string departureName, DateTime departureTime, string? destinationName, DateTime? destinationTime, bool isDestinationVisible, string? trainName, string? trainDetail, int? price, IEnumerable<RouteMessage> messages, RouteState routeState, UserInfo userInfo
            , IKVPStore kVPStore, string stateTableKey, string messageTableKey, string destinationVisibleTableKey)
        {
            Id = id;
            DisplayName = displayName;
            Day = day;
            DepartureName = departureName;
            DepartureTime = departureTime;
            DestinationName = destinationName;
            DestinationTime = destinationTime;
            IsDestinationVisible = isDestinationVisible;
            TrainName = trainName;
            TrainDetail = trainDetail;
            Price = price;
            this.messages = messages.Select((x, i) => new RouteMessage(x.Line, x.Message, x.IsVisible, value => SaveMessageVisiability(i, value))).ToList();

            RouteState = routeState;
            UserInfo = userInfo;

            this.kVPStore = kVPStore;
            this.stateTableKey = stateTableKey;
            this.messageTableKey = messageTableKey;
            this.destinationVisibleTableKey = destinationVisibleTableKey;
        }

        private readonly IKVPStore kVPStore;
        private readonly string stateTableKey;
        private readonly string messageTableKey;
        private readonly string destinationVisibleTableKey;

        private void SaveMessageVisiability(int index, bool value)
        {
            var tableName = string.Format(messageTableKey, index);
            kVPStore.Set(tableName, Id, value.ToString());
        }

        /// <summary>
        /// この行程の一意の ID を取得します。
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// この行程の表示名を取得します。
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// この行程に関連付けられた日付を取得します。
        /// </summary>
        public int Day { get; }

        /// <summary>
        /// 出発地点名を取得します。
        /// </summary>
        public string DepartureName { get; }

        /// <summary>
        /// 出発時刻を取得します。
        /// </summary>
        public DateTime DepartureTime { get; }

        /// <summary>
        /// 到着地点名を取得します。
        /// </summary>
        public string? DestinationName { get; }

        /// <summary>
        /// 到着時刻を取得します。
        /// </summary>
        public DateTime? DestinationTime { get; }

        /// <summary>
        /// 目的地を表示するかどうかを表す値を取得します。
        /// </summary>
        /// <remarks>
        /// このプロパティを設定するには <see cref="SetIsDestinationVisible(bool)"/> を使用します。
        /// </remarks>
        public bool IsDestinationVisible { get; private set; }

        /// <summary>
        /// 目的地を表示するかどうかを表す値を設定し、永続化します。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public void SetIsDestinationVisible(bool value)
        {
            if (IsDestinationVisible == value)
            {
                return;
            }
            kVPStore.Set(destinationVisibleTableKey, Id, value.ToString());
            IsDestinationVisible = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDestinationVisible)));
        }

        /// <summary>
        /// 移動手段名を取得します。
        /// </summary>
        public string? TrainName { get; }

        /// <summary>
        /// 移動手段詳細情報を取得します。
        /// </summary>
        public string? TrainDetail { get; }

        /// <summary>
        /// 移動費用を取得します。
        /// </summary>
        public int? Price { get; }

        private readonly List<RouteMessage> messages;

        /// <summary>
        /// この行程に関連付けられたメッセージのリストを取得します。
        /// </summary>
        public IReadOnlyList<RouteMessage> Messages => messages;

        /// <summary>
        /// この行程の状態を取得します。
        /// </summary>
        /// <remarks>
        /// 状態を設定するには <see cref="SetRouteState"/> を使用します。
        /// </remarks>
        public RouteState RouteState { get; private set; }

        /// <summary>
        /// この行程の状態を設定し、永続化します。
        /// </summary>
        /// <returns></returns>
        public void SetRouteState(RouteState value)
        {
            if (RouteState == value)
            {
                return;
            }

            kVPStore.Set(stateTableKey, Id, value.GetJsonValue());
            RouteState = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RouteState)));
        }

        /// <summary>
        /// この行程に関連付けられたユーザー権限情報を取得します。
        /// </summary>
        public UserInfo UserInfo { get; }

        /// <summary>
        /// この行程のプロパティが変更されたときに発生するイベント。
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}