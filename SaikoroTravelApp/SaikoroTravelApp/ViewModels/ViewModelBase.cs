using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SaikoroTravelApp
{
    /// <summary>
    /// ViewModelの共通基底実装を提供します。
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {

        public ViewModelBase()
        {

        }

        /// <summary>
        /// プロパティの値を設定します。
        /// </summary>
        /// <typeparam name="T">プロパティの値の型。</typeparam>
        /// <param name="p">設定対象のプロパティ。</param>
        /// <param name="value">設定する値。</param>
        /// <param name="propertyName">設定対象のプロパティの名前。呼び出し元の名前と同じ場合は省略できます。</param>
        protected void SetProperty<T>(ref T p, T value, [CallerMemberName] string propertyName = null)
        {
            if (p == null)
            {
                if (value == null)
                {
                    return;
                }
            }
            else
            {
                if (p.Equals(value))
                {
                    return;
                }
            }
            p = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// プロパティの値が変更された場合に発生するイベント。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// プロパティの変更を通知します。
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
