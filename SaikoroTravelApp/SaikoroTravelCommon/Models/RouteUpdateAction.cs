using System.Linq;

namespace SaikoroTravelCommon.Models
{
    /// <summary>
    /// 行程を更新するアクションを表現します。
    /// </summary>
    public abstract class RouteUpdateAction
    {
        protected RouteUpdateAction(string targetId)
        {
            TargetId = targetId;
        }

        /// <summary>
        /// 更新対象の行程の ID を取得します。
        /// </summary>
        public string TargetId { get; }

        /// <summary>
        /// 更新を適用します。
        /// </summary>
        /// <param name="manager">更新対象の行程を管理する <see cref="RouteManager"/>。</param>
        /// <returns></returns>
        public abstract void Apply(RouteManager manager);
    }

    /// <summary>
    /// 行程の <see cref="Route.IsDestinationVisible"/> プロパティの値を更新するアクションを表現します。
    /// </summary>
    public class RouteDestinationVisibleUpdateAction : RouteUpdateAction
    {
        /// <summary>
        /// <see cref="RouteDestinationVisibleUpdateAction"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="targetId">更新対象の行程の ID。</param>
        /// <param name="value">更新後にセットする値。</param>
        public RouteDestinationVisibleUpdateAction(string targetId, bool value) : base(targetId)
        {
            Value = value;
        }

        /// <summary>
        /// このアクションで設定する値を取得します。
        /// </summary>
        public bool Value { get; }

        /// <inheritdoc />
        public override void Apply(RouteManager manager)
        {
            manager.FindById(TargetId).SetIsDestinationVisible(Value);
        }
    }

    /// <summary>
    /// 行程の <see cref="Route.Messages"/> プロパティの <see cref="RouteMessage.IsVisible"/> プロパティの値を更新するアクションを表現します。
    /// </summary>
    public class RouteMessageVisibleUpdateAction : RouteUpdateAction
    {
        /// <summary>
        /// <see cref="RouteMessageVisibleUpdateAction"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="targetId">更新対象の行程の ID。</param>
        /// <param name="value">更新後にセットする値。</param>
        /// <param name="line">更新対象のメッセージの行 (1 始まりのインデックス) 。</param>
        public RouteMessageVisibleUpdateAction(string targetId, bool value, int line) : base(targetId)
        {
            Value = value;
            Line = line;
        }

        /// <summary>
        /// 更新後にセットする値を取得します。
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// 更新対象のメッセージの行 (1 始まりのインデックス) を取得します。
        /// </summary>
        public int Line { get; }

        /// <inheritdoc />
        public override void Apply(RouteManager manager)
        {
            manager.FindById(TargetId).Messages.Single(x => x.Line == Line).SetIsVisible(Value);
        }
    }

    /// <summary>
    /// 行程の <see cref="Route.RouteState"/> プロパティの値をを更新するアクションを表現します。
    /// </summary>
    public class RouteStateUpdateAction : RouteUpdateAction
    {
        /// <summary>
        /// <see cref="RouteStateUpdateAction"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="targetId">更新対象の行程の ID。</param>
        /// <param name="value">更新後にセットする値。</param>
        public RouteStateUpdateAction(string targetId, RouteState value) : base(targetId)
        {
            Value = value;
        }

        /// <summary>
        /// 更新後にセットする値を取得します。
        /// </summary>
        public RouteState Value { get; }

        /// <inheritdoc />
        public override void Apply(RouteManager manager)
        {
            manager.FindById(TargetId).SetRouteState(Value);
        }
    }
}
