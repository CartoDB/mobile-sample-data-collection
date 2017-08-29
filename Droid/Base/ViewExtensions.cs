
using System;
using Android.Animation;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace data.collection.Droid
{
    public static class ViewExtensions
    {
        public static void SetFrame(this View view, int x, int y, int w, int h)
        {
            var parameters = new RelativeLayout.LayoutParams(w, h);
            parameters.LeftMargin = x;
            parameters.TopMargin = y;

            view.LayoutParameters = parameters;
        }

        public static void MatchParent(this View view)
        {
            var parent = ViewGroup.LayoutParams.MatchParent;
            var parameters = new RelativeLayout.LayoutParams(parent, parent);
            view.LayoutParameters = parameters;
        }

        const int Lollipop = 21;
        const int JellyBean = 16;

        public static bool IsLollipopOrHigher(this View view)
        {
            return (int)Build.VERSION.SdkInt >= Lollipop;
        }

        public static bool IsJellyBeanOrHigher(this View view)
        {
            return (int)Build.VERSION.SdkInt >= JellyBean;
        }

        public static void SetBackground(this View view, Color color)
        {
            var drawable = new GradientDrawable();
            drawable.SetColor(color);

            if (view.IsJellyBeanOrHigher())
            {
                view.Background = drawable;
            }
            else
            {
                view.SetBackgroundColor(color);
            }
        }

        public static void SetCornerRadius(this View view, int radius)
        {
            if (view.Background is GradientDrawable)
            {
                (view.Background as GradientDrawable).SetCornerRadius(radius);
            }
        }
		public static void SetBorderColor(this View view, int width, Color color)
		{
			if (view.Background is GradientDrawable)
			{
                (view.Background as GradientDrawable).SetStroke(width, color);
			}
		}

        public static void RemoveCustomCursorColor(this EditText view)
        {
            IntPtr viewClass = JNIEnv.FindClass(typeof(TextView));
            IntPtr cursorProperty = JNIEnv.GetFieldID(viewClass, "mCursorDrawableRes", "I");
			JNIEnv.SetField(view.Handle, cursorProperty, 0);
		}

        public static void AnimateY(this View view, float to, int duration = 200, Action complete = null)
        {
            var animator = ObjectAnimator.OfFloat(view, "y", to);
            animator.SetDuration(duration);
            animator.Start();

            animator.AnimationEnd += (object sender, EventArgs e) =>
            {
                if (complete != null)
                {
                    complete();
                }
                animator.Dispose();
            };
        }

    }
}
