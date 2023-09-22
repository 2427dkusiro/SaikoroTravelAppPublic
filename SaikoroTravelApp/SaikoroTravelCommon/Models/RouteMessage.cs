using System;

namespace SaikoroTravelCommon.Models
{

    /// <summary>
    /// 行程に付与されたメッセージを表現します。
    /// </summary>
    public class RouteMessage
    {
        /// <summary>
        /// <see cref="RouteMessage"/> クラスの新しいインスタンスを、可視性変更不可能の状態で初期化します。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="message"></param>
        /// <param name="isVisible"></param>
        public RouteMessage(int line, string message, bool isVisible)
        {
            Line = line;
            Message = message;
            IsVisible = isVisible;
        }

        /// <summary>
        /// <see cref="RouteMessage"/> クラスの新しいインスタンスを、可視性変更を永続化する関数を指定して初期化します。
        /// </summary>
        /// <param name="line"></param>
        /// <param name="message"></param>
        /// <param name="isVisible"></param>
        /// <param name="saveAction"></param>
        public RouteMessage(int line, string message, bool isVisible, Action<bool> saveAction)
        {
            Line = line;
            Message = message;
            IsVisible = isVisible;
            this.saveAction = saveAction;
        }

        /// <summary>
        /// このメッセージの 1 始まりの行数を取得します。
        /// </summary>
        public int Line { get; }

        /// <summary>
        /// このメッセージの本文を取得します。
        /// </summary>
        public string Message { get; }

        private readonly Action<bool> saveAction;

        /// <summary>
        /// このメッセージの可視性を表す値を取得します。
        /// </summary>
        /// <remarks>
        /// このプロパティを設定するには <see cref="SetIsVisible"/> を使用します。
        /// </remarks>
        public bool IsVisible { get; private set; }

        public void SetIsVisible(bool value)
        {
            if (IsVisible == value)
            {
                return;
            }
            if (saveAction is null)
            {
                throw new InvalidOperationException("saveAction not set");
            }

            saveAction(value);
            IsVisible = value;
            VisiabilityChanged?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 可視性の値が変更されたときに発生するイベント。
        /// </summary>
        public event EventHandler<EventArgs>? VisiabilityChanged;
    }
}