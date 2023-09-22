namespace SaikoroTravelCommon.Models
{
    /// <summary>
    /// 行程の状態を表現します。
    /// </summary>
    public enum RouteState
    {
        /// <summary>
        /// 破棄状態
        /// </summary>
        Abandoned,

        /// <summary>
        /// 未確定状態
        /// </summary>
        Unsettled,

        /// <summary>
        /// 有効化待ち状態
        /// </summary>
        WaitingForActivation,

        /// <summary>
        /// 有効化状態
        /// </summary>
        Activated,

        /// <summary>
        /// 実行済み状態
        /// </summary>
        Executed
    }
}