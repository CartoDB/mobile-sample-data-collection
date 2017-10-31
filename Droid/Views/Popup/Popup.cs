using System;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Views.Animations;
using Android.Widget;

namespace data.collection.Droid.Views.Popup
{
    public class Popup : BaseView
    {
        int hiddenY, smallVisibleY, fullVisibleY = -1;

        public bool IsVisible
        {
            get { return !Frame.Y.Equals(hiddenY); }
        }

        public Popup(Context context) : base(context)
        {
            SetBackgroundColor(Color.White);

            if (this.IsLollipopOrHigher())
            {
                Elevation = 5.0f;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
        }

        public void SetLocations(int hiddenY, int smallVisibleY, int fullVisibleY)
        {
            this.hiddenY = hiddenY;
            this.smallVisibleY = smallVisibleY;
            this.fullVisibleY = fullVisibleY;
        }

        public void ShowFull()
        {
            BringToFront();
            AnimateY(fullVisibleY);
        }

        public void ShowSmall()
        {
            BringToFront();
            AnimateY(smallVisibleY);
        }

        public void Hide()
        {
            BringToFront();
            AnimateY(hiddenY);
        }

        void AnimateY(int to, long duration = 200)
        {
            // TODO Create a container and animate that. 
            // Android doesn't really handle off-screen animations very well.
            var from = Frame.Y;
            var animator = ObjectAnimator.OfFloat(this, "y", from, to);
            animator.SetDuration(duration);
            animator.Start();

            animator.AnimationEnd += (object sender, EventArgs e) => {
                UpdateY(to);
                TranslationY = 0;
            };
        }

    }
}
