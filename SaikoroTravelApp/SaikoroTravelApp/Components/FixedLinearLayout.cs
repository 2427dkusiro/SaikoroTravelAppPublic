using Android.Content;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace SaikoroTravelApp.Components
{
    /// <summary>
    /// 指定のアスペクト比で表示される線形レイアウトを表現します。
    /// </summary>
    internal abstract class FixedLinearLayout : LinearLayout
    {
        /// <summary>
        /// <see cref="FixedLinearLayout"/> クラスの新しいインスタンスを取得します。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attributeSet"></param>
        public FixedLinearLayout(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
        {

        }

        /// <summary>
        /// 希望する横方向の長さの重みを取得します。
        /// </summary>
        protected abstract int UnitWidth { get; }

        /// <summary>
        /// 希望する縦方向の長さの重みを取得します。
        /// </summary>
        protected abstract int UnitHeight { get; }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            var measureWidth = MeasureSpec.GetSize(widthMeasureSpec);
            MeasureSpecMode widthMode = MeasureSpec.GetMode(widthMeasureSpec);
            var measureHeight = MeasureSpec.GetSize(heightMeasureSpec);
            MeasureSpecMode heightMode = MeasureSpec.GetMode(heightMeasureSpec);

            int calcMeasureSpecWidth;
            int calcMeasureSpecHeight;

            void FromWidth()
            {
                var height = measureWidth * UnitHeight / UnitWidth;
                calcMeasureSpecWidth = MeasureSpec.MakeMeasureSpec(measureWidth, MeasureSpecMode.Exactly);
                calcMeasureSpecHeight = MeasureSpec.MakeMeasureSpec(height, MeasureSpecMode.Exactly);
            }

            void FromHeight()
            {
                var width = measureHeight * UnitWidth / UnitHeight;
                calcMeasureSpecWidth = MeasureSpec.MakeMeasureSpec(width, MeasureSpecMode.Exactly);
                calcMeasureSpecHeight = MeasureSpec.MakeMeasureSpec(measureHeight, MeasureSpecMode.Exactly);
            }

            if (widthMode == MeasureSpecMode.Exactly && heightMode != MeasureSpecMode.Exactly)
            {
                FromWidth();
            }
            else if (widthMode != MeasureSpecMode.Exactly && heightMode == MeasureSpecMode.Exactly)
            {
                FromHeight();
            }
            else if (measureWidth * UnitHeight > measureHeight * UnitWidth)
            {
                // 領域は基準より横長
                FromHeight();
            }
            else
            {
                // 領域は基準より縦長
                FromWidth();
            }

            base.OnMeasure(calcMeasureSpecWidth, calcMeasureSpecHeight);
        }
    }
}