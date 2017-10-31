using System;
using Android.Animation;
using Android.Content;
using Android.Graphics;

namespace data.collection.Droid.Views.Popup
{
    public class Popup : BaseView
    {
        int hiddenY, visibleY = -1;

        public Popup(Context context) : base(context)
        {
            SetBackgroundColor(Color.White);

            if (this.IsLollipopOrHigher())
            {
                Elevation = 5.0f;
            }
        }

        public void Show()
        {
            BringToFront();
            Visibility = Android.Views.ViewStates.Visible;

            AnimateY(visibleY);
        }

        public void Hide(long duration = 200)
        {
            AnimateY(hiddenY, duration);
        }

        void Hide(object sender, EventArgs e)
        {
            Hide();
        }

        void AnimateY(int to, long duration = 200)
        {
            var animator = ObjectAnimator.OfFloat(this, "y", to);
            animator.SetDuration(duration);
            animator.Start();

            animator.AnimationEnd += (object sender, EventArgs e) => {
                if (to == hiddenY)
                {
                    Visibility = Android.Views.ViewStates.Gone;
                }

                animator.Dispose();
            };
        }
    }
}
