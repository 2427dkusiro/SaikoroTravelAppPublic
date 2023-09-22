using Android.Animation;
using Android.Content;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using SaikoroTravelApp.ViewModels;

using System;
using System.Linq;

namespace SaikoroTravelApp.Components
{
    /// <summary>
    /// 指示を表示するUIコンポーネントを表現します。
    /// </summary>
    internal class InstructionView : FixedLinearLayout
    {
        private readonly FrameLayout mainLayout;
        private readonly LinearLayout contentLayout;
        private readonly LinearLayout animationLayout;

        private readonly LinearLayout leftMargin;
        private readonly LinearLayout rightMargin;
        private readonly LinearLayout animationContentFrame;

        private readonly TextView text;

        /// <summary>
        /// <see cref="InstructionView"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="attributeSet"></param>
        public InstructionView(Context context, IAttributeSet attributeSet) : base(context, attributeSet)
        {
            _ = Inflate(Context, Resource.Layout.component_instruction, this);

            mainLayout = FindViewById<FrameLayout>(Resource.Id.InstructionMainLayout);
            text = FindViewById<TextView>(Resource.Id.InstructionText);

            contentLayout = FindViewById<LinearLayout>(Resource.Id.InstructionContentLayout);
            animationLayout = FindViewById<LinearLayout>(Resource.Id.InstructionAnimationLayout);

            leftMargin = FindViewById<LinearLayout>(Resource.Id.InstructionAnimationMarginLeft);
            rightMargin = FindViewById<LinearLayout>(Resource.Id.InstructionAnimationMarginRight);
            animationContentFrame = FindViewById<LinearLayout>(Resource.Id.InstructionAnimationCenter);

            mainLayout.Click += MainLayout_Click;
        }

        private void MainLayout_Click(object sender, EventArgs e)
        {
            if (!(viewModel?.IsOpend ?? true))
            {
                Open();
            }
        }

        private InstructionViewModel viewModel;

        /// <summary>
        /// このコンポーネントに関連付けられたビューモデルを取得または設定します。
        /// </summary>
        public InstructionViewModel ViewModel
        {
            get => viewModel;
            set
            {
                if (ReferenceEquals(ViewModel, value))
                {
                    return;
                }

                if (!(viewModel is null))
                {
                    viewModel.PropertyChanged -= ViewModelPropertyChanged;
                }
                value.PropertyChanged += ViewModelPropertyChanged;

                if (viewModel != value)
                {
                    viewModel = value;
                    Render();
                }
                else
                {
                    viewModel = value;
                }
            }
        }

        private void ViewModelPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // 更新された要素のみ再描画するほうがなお良い
            Render();
        }

        private void Render()
        {
            if (viewModel.IsOpend)
            {
                contentLayout.Visibility = ViewStates.Visible;
                animationContentFrame.Visibility = ViewStates.Gone;
                mainLayout.RemoveView(animationLayout);
            }
            else
            {
                contentLayout.Visibility = ViewStates.Gone;
                animationLayout.Visibility = ViewStates.Visible;
                ResetAnimation();
            }

            text.Text = viewModel.Content;
            text.SetMaxLines(viewModel.Content.Count(x => x == '\n') + 1);
        }

        private const int animationLengthMove = 1500;
        private const int animationLengthOpen = 3500;

        private bool isOpening = false;

        /// <summary>
        /// 指示を開くアニメーションを再生し、開きます。
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Open()
        {
            if (isOpening)
            {
                return;
            }
            isOpening = true;

            if (viewModel is null)
            {
                throw new InvalidOperationException("ViewModel is null.");
            }

            viewModel.NotifyAnimationStarted(this);
            ResetAnimation();
            contentLayout.Visibility = ViewStates.Visible;

            PlayOpenAnimation();
        }

        private void PlayOpenAnimation()
        {
            var leftParam = leftMargin.LayoutParameters as LayoutParams;
            var rightParam = rightMargin.LayoutParameters as LayoutParams;
            var contextParam = animationContentFrame.LayoutParameters as LayoutParams;

            var leftAnimator = ValueAnimator.OfFloat(1f, 0f);
            _ = leftAnimator.SetDuration(animationLengthMove);
            leftAnimator.Update += (sender, e) =>
            {
                leftParam.Weight = (float)e.Animation.AnimatedValue.JavaCast<Java.Lang.Float>();
                leftMargin.LayoutParameters = leftParam;
            };

            var rightAnimator = ValueAnimator.OfFloat(1f, 0f);
            _ = rightAnimator.SetDuration(animationLengthOpen);
            rightAnimator.Update += (sender, e) =>
            {
                var value = (float)e.Animation.AnimatedValue.JavaCast<Java.Lang.Float>();
                rightParam.Weight = value;
                rightMargin.LayoutParameters = rightParam;
                contextParam.Weight = 1 - value;
                animationContentFrame.LayoutParameters = contextParam;
            };

            leftAnimator.AnimationEnd += (sender, e) => rightAnimator.Start();
            rightAnimator.AnimationEnd += (sender, e) =>
            {
                mainLayout.RemoveView(animationLayout);
                isOpening = false;
                viewModel.NotifyOpened(this, false);
            };

            leftAnimator.Start();
        }

        private void ResetAnimation()
        {
            if (mainLayout.IndexOfChild(animationLayout) == -1)
            {
                mainLayout.AddView(animationLayout);
            }

            var leftParam = leftMargin.LayoutParameters as LayoutParams;
            var rightParam = rightMargin.LayoutParameters as LayoutParams;
            var contextParam = animationContentFrame.LayoutParameters as LayoutParams;
            leftParam.Weight = 1;
            rightParam.Weight = 1;
            contextParam.Weight = 0;
            leftMargin.LayoutParameters = leftParam;
            rightMargin.LayoutParameters = rightParam;
            animationContentFrame.LayoutParameters = contextParam;
        }

        protected override int UnitWidth => 3800;

        protected override int UnitHeight => 2000;
    }
}