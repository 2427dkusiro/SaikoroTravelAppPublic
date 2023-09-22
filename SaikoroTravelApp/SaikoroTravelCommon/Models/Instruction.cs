using SaikoroTravelCommon.IO;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace SaikoroTravelCommon.Models
{
    /// <summary>
    /// 指示を表現します。
    /// </summary>
    public class Instruction : INotifyPropertyChanged
    {
        /// <summary>
        /// <see cref="Instruction"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="day"></param>
        /// <param name="desiredNotificationTime"></param>
        /// <param name="limitTime"></param>
        /// <param name="content"></param>
        /// <param name="kVPStore"></param>
        /// <param name="tableKey"></param>
        /// <param name="instructionState"></param>
        /// <param name="userInfo"></param>
        /// <param name="routeUpdates"></param>
        public Instruction(string id, int day, DateTime desiredNotificationTime, DateTime limitTime, string content, IKVPStore kVPStore, string tableKey, RouteState instructionState, UserInfo userInfo, IEnumerable<RouteUpdateAction> routeUpdates)
        {
            Id = id;
            Day = day;
            DesiredNotificationTime = desiredNotificationTime;
            LimitTime = limitTime;
            Content = content;
            this.kVPStore = kVPStore;
            this.tableKey = tableKey;
            InstructionState = instructionState;
            UserInfo = userInfo;
            this.routeUpdates = routeUpdates.ToList();
        }

        /// <summary>
        /// この指示の一意の ID を取得します。
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// この指示の日付を取得します。
        /// </summary>
        public int Day { get; }

        /// <summary>
        /// この指示が通知される希望時刻を取得します。
        /// </summary>
        public DateTime DesiredNotificationTime { get; }

        /// <summary>
        /// この指示が実行されるべき締め切り時刻を取得します。
        /// </summary>
        public DateTime LimitTime { get; }

        /// <summary>
        /// この指示の内容を取得します。
        /// </summary>
        public string Content { get; }

        private readonly IKVPStore kVPStore;

        private readonly string tableKey;

        /// <summary>
        /// この指示の状態を取得します。
        /// </summary>
        /// <remarks>
        /// 状態を設定するには <see cref="SetState(RouteState)"/> を使用します。
        /// </remarks>
        public RouteState InstructionState { get; private set; }

        /// <summary>
        /// この指示の状態を変更し、その変更を永続化します。
        /// </summary>
        /// <param name="value">変更する値。</param>
        /// <returns></returns>
        public void SetState(RouteState value)
        {
            if (InstructionState == value)
            {
                return;
            }

            kVPStore.Set(tableKey, Id, value.GetJsonValue());
            InstructionState = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InstructionState)));
        }

        /// <summary>
        /// この指示のユーザー権限情報を取得します。
        /// </summary>
        public UserInfo UserInfo { get; }

        private readonly List<RouteUpdateAction> routeUpdates;

        /// <summary>
        /// この指示が開かれたときに実行すべき行程の更新を取得します。
        /// </summary>
        public IReadOnlyList<RouteUpdateAction> RouteUpdates => routeUpdates;

        /// <summary>
        /// この指示のプロパティが変更されたときに発生するイベント。
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        public override int GetHashCode()
        {
            int upperMask;
            string numStr;
            if (Id.Contains('-'))
            {
                var index = Id.IndexOf('-');
                var prefix = Id[0..index];
                numStr = Id[(index + 1)..];
                upperMask = prefix.ToUpper() switch
                {
                    "**" => 1,
                    "***" => 2,
                    "****" => 4,
                    _ => 8
                } << 16;
            }
            else
            {
                upperMask = 0;
                numStr = Id;
            }

            var num = decimal.Parse(numStr) * 10;
            var intNum = (int)num;
            Debug.Assert(num == intNum);
            return upperMask | intNum;
        }
    }
}
