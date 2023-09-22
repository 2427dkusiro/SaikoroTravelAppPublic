using System;
#nullable enable

namespace SaikoroTravelApp.ViewModels
{
    /// <summary>
    /// 指示ビューのビューモデルを表現します。
    /// </summary>
    internal class InstructionViewModel : ViewModelBase
    {
        private string? content;

        /// <summary>
        /// 表示するコンテンツを取得または設定します。
        /// </summary>
        public string? Content
        {
            get => content;
            set => SetProperty(ref content, value);
        }

        private bool isOpend;

        /// <summary>
        /// 指示が開かれた状態で表示されるかどうかを表す値を取得または設定します。
        /// </summary>
        public bool IsOpend
        {
            get => isOpend;
            set => SetProperty(ref isOpend, value);
        }

        /// <summary>
        /// 指示が開かれた場合に発生するイベント。
        /// </summary>
        public event EventHandler<EventArgs>? InstructionOpened;

        public event EventHandler<EventArgs>? AnimationStarted;

        /// <summary>
        /// 指示が開かれたことを通知します。
        /// </summary>
        /// <param name="sender">イベントソース。</param>
        /// <param name="raisePropertyChanged">プロパティ変更イベントを発生させるかどうか。</param>
        public void NotifyOpened(object sender, bool raisePropertyChanged)
        {
            if (raisePropertyChanged)
            {
                IsOpend = true;
            }
            else
            {
                isOpend = true;
            }
            InstructionOpened?.Invoke(sender, new EventArgs());
        }

        public void NotifyAnimationStarted(object sender)
        {
            AnimationStarted?.Invoke(sender, new EventArgs());
        }
    }
}