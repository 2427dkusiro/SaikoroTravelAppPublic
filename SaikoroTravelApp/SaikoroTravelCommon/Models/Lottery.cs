using SaikoroTravelCommon.IO;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace SaikoroTravelCommon.Models
{
    /// <summary>
    /// 抽選を表現します。
    /// </summary>
    public class Lottery : INotifyPropertyChanged
    {
        /// <summary>
        /// <see cref="Lottery"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="id"></param>
        /// <param name="title"></param>
        /// <param name="day"></param>
        /// <param name="desiredNotificationTime"></param>
        /// <param name="limitTime"></param>
        /// <param name="lotteryState"></param>
        /// <param name="user"></param>
        /// <param name="lotteryOptions"></param>
        public Lottery(string id, string title, int day, DateTime desiredNotificationTime, DateTime limitTime, RouteState lotteryState, UserInfo user, IEnumerable<LotteryOption> lotteryOptions, IKVPStore accesser, string tableName)
        {
            Id = id;
            Title = title;
            Day = day;
            DesiredNotificationTime = desiredNotificationTime;
            LimitTime = limitTime;
            LotteryState = lotteryState;
            UserInfo = user;
            this.lotteryOptions = lotteryOptions.ToList();
            this.accesser = accesser;
            this.tableName = tableName;
        }

        private readonly IKVPStore accesser;
        private readonly string tableName;

        /// <summary>
        /// この抽選の一意の ID を取得します。 
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// この抽選のタイトルを取得します。
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// この抽選に関連付けられた日数を取得します。
        /// </summary>
        public int Day { get; }

        /// <summary>
        /// この抽選の通知の送信希望時刻を取得します。
        /// </summary>
        public DateTime DesiredNotificationTime { get; }

        /// <summary>
        /// この抽選が実行されるべき締め切り時刻を取得します。
        /// </summary>
        public DateTime LimitTime { get; }

        /// <summary>
        /// この抽選の状態を取得します。
        /// </summary>
        public RouteState LotteryState { get; private set; }

        public void SetLotteryState(RouteState value)
        {
            if (LotteryState == value)
            {
                return;
            }
            accesser.Set(tableName, Id, value.GetJsonValue());
            LotteryState = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LotteryState)));
        }

        /// <summary>
        /// この抽選のユーザー権限情報を取得します。
        /// </summary>
        public UserInfo UserInfo { get; }

        private readonly List<LotteryOption> lotteryOptions;

        public IReadOnlyList<LotteryOption> LotteryOptions => lotteryOptions;

        public event PropertyChangedEventHandler? PropertyChanged;

        public override int GetHashCode()
        {
            var num = decimal.Parse(Id) * 10;
            var intNum = (int)num;
            Debug.Assert(num == intNum);
            return intNum;
        }
    }

    /// <summary>
    /// 抽選の選択肢を表現します。
    /// </summary>
    public class LotteryOption
    {
        public LotteryOption(int number, bool selectable, string title, string subTitle, IEnumerable<InstructionUpdateAction> instructionUpdates)
        {
            Number = number;
            Selectable = selectable;
            Title = title;
            SubTitle = subTitle;
            this.instructionUpdates = instructionUpdates.ToList();
        }

        /// <summary>
        /// この選択肢に関連付けられた出目を取得します。
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// この選択肢が選択可能かどうかを表す値を取得します。
        /// </summary>
        public bool Selectable { get; }

        /// <summary>
        /// この選択肢のタイトルを取得します。
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// この選択肢のサブタイトルを取得します。
        /// </summary>
        public string SubTitle { get; }

        private readonly List<InstructionUpdateAction> instructionUpdates;

        /// <summary>
        /// この選択肢が選択されたときに実行されるべき指示の更新アクションを取得します。
        /// </summary>
        public IReadOnlyList<InstructionUpdateAction> InstructionUpdates => instructionUpdates;
    }

    /// <summary>
    /// 指示を更新するアクションを表現します。
    /// </summary>
    public abstract class InstructionUpdateAction
    {
        /// <summary>
        /// このコンストラクタを継承します。
        /// </summary>
        /// <param name="targetId">更新対象の指示の ID。</param>
        protected InstructionUpdateAction(string targetId)
        {
            TargetId = targetId;
        }

        /// <summary>
        /// 更新対象の指示の ID を取得します。
        /// </summary>
        public string TargetId { get; }

        /// <summary>
        /// 指示を更新します。
        /// </summary>
        /// <param name="instructionManager">更新対象の指示を管理する <see cref="InstructionManager"/>。</param>
        public abstract void Apply(InstructionManager instructionManager);
    }

    /// <summary>
    /// 指示の状態を更新するアクションを表現します。
    /// </summary>
    public class InstructionStateUpdateAction : InstructionUpdateAction
    {
        /// <summary>
        /// <see cref="InstructionStateUpdateAction"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="targetId">更新対象の指示の ID。</param>
        /// <param name="value">更新する値。</param>
        public InstructionStateUpdateAction(string targetId, RouteState value) : base(targetId)
        {
            Value = value;
        }

        /// <summary>
        /// 更新する値を取得します。
        /// </summary>
        public RouteState Value { get; }

        /// <inheritdoc />
        public override void Apply(InstructionManager instructionManager)
        {
            instructionManager.FindById(TargetId).SetState(Value);
        }
    }
}
