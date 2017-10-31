
using System;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.OS;

namespace data.collection.Droid
{
    public class SlideInPopup : BaseView
    {
        public BaseView TransparentArea { get; private set; }
        PopupView popup;

        int hiddenY, visibleY = -1;

        BaseView content;

        public PopupHeader Header { get { return popup.Header; } }

        public bool IsVisible
        { 
            get { return popup.Frame.Y.Equals(visibleY); }
        }

        public void ShowBackButton()
        {
            Header.BackButton.Visibility = Android.Views.ViewStates.Visible;
        }

		public void HideBackButton()
		{
            Header.BackButton.Visibility = Android.Views.ViewStates.Gone;
		}

        public SlideInPopup(Context context, int backIcon, int closeIcon) : base(context)
        {
            TransparentArea = new BaseView(context);
            TransparentArea.SetBackgroundColor(Color.Black);
            TransparentArea.Alpha = 0.0f;
            AddView(TransparentArea);

            popup = new PopupView(context, backIcon, closeIcon);
            AddView(popup);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                Elevation = 11.0f;
            }
        }

        public override void LayoutSubviews()
        {
            int x = 0;
            int y = 0;
            int w = Frame.W;
            int h = Frame.H;

            TransparentArea.SetFrame(x, y, w, h);

            hiddenY = h;
            visibleY = h - (h / 5 * 3);

            if (IsLandscape || IsLargeTablet)
            {
                w = (int)(400 * Density);
                visibleY = 0;
            }

            if (!IsLandscape && IsLargeTablet)
            {
                h = Frame.W;
                visibleY = Frame.H - h;
            }

            y = visibleY;
            popup.Frame = new CGRect(x, y, w, h);

            Hide(0);
        }

        public void SetPopupContent(BaseView content)
        {
            if (this.content != null)
            {
                popup.RemoveView(this.content);
                this.content = null;
            }

            this.content = content;
            popup.AddView(content);

            int x = 0;
            int y = popup.Header.TotalHeight;
            int w = popup.Frame.W;
            int h = popup.Frame.H - popup.Header.TotalHeight;

            content.Frame = new CGRect(x, y, w, h);
        }

        public void Show()
        {
            BringToFront();
            Visibility = Android.Views.ViewStates.Visible;

            AnimateAlpha(0.5f);
            AnimateY(visibleY);

            TransparentArea.Click += Hide;
            popup.Header.CloseButton.Click += Hide;
        }

        public void Hide(long duration = 200)
        {
            AnimateAlpha(0.0f, duration);
            AnimateY(hiddenY, duration);

			TransparentArea.Click -= Hide;
			popup.Header.CloseButton.Click -= Hide;
        }

        public EventHandler<EventArgs> Closed;
        void Hide(object sender, EventArgs e)
        {
            Hide();
            Closed?.Invoke(this, EventArgs.Empty);
        }

        void AnimateAlpha(float to, long duration = 200)
        {
            var animator = ObjectAnimator.OfFloat(TransparentArea, "Alpha", to);
            animator.SetDuration(duration);
            animator.Start();
        }

        void AnimateY(int to, long duration = 200)
        {
            var animator = ObjectAnimator.OfFloat(popup, "y", to);
            animator.SetDuration(duration);
            animator.Start();

            animator.AnimationEnd += (object sender, EventArgs e) => {
                if (to == hiddenY)
                {
                    Visibility = Android.Views.ViewStates.Gone;
                }
                popup.SetInternalFrame(popup.Frame.Y, to, popup.Frame.W, popup.Frame.H);
                animator.Dispose();
            };
        }

    }
}
