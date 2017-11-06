using System;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Views.InputMethods;
using Android.Widget;

namespace data.collection.Droid
{
    public class BaseView : RelativeLayout
    {
        CGRect frame = CGRect.Empty;
        public CGRect Frame 
        {
            get { return frame; }
            set { 
                frame = value;

				var parameters = new RelativeLayout.LayoutParams(frame.W, frame.H);
                parameters.LeftMargin = frame.X;
                parameters.TopMargin = frame.Y;

				LayoutParameters = parameters;

                LayoutSubviews();
            }
        }

        public void SetInternalFrame(int x, int y, int width, int height)
		{
            frame = new CGRect(x, y, width, height);
		}

        public void UpdateY(int y)
        {
            Frame = new CGRect(Frame.X, y, Frame.W, Frame.H);
        }

        public void UpdateInternalX(int x)
        {
            SetInternalFrame(x, Frame.Y, Frame.W, Frame.H);
        }

        public float Density 
        { 
            get { return Context.Resources.DisplayMetrics.Density; } 
        }

        public int UsableHeight
        {
            get
            {
                int total = Resources.DisplayMetrics.HeightPixels;
                return total - (NavigationBarHeight + StatusBarHeight /*+ ActionBarHeight*/);
            }
        }

        public int NavigationBarHeight { get { return GetSize("navigation_bar_height"); } }

		public int StatusBarHeight { get { return GetSize("status_bar_height"); } }

        int GetSize(string of)
        {
            var result = 0;
            var resourceId = Resources.GetIdentifier(of, "dimen", "android");

            if (resourceId > 0)
            {
                result = Resources.GetDimensionPixelSize(resourceId);    
            }

            return result;
        }

		public int ActionBarHeight
		{
			get
			{
                var tv = new TypedValue();
                var id = Android.Resource.Attribute.ActionBarSize;
                Context.Theme.ResolveAttribute(id, tv, true);
                return Resources.GetDimensionPixelSize(tv.ResourceId);
			}
		}

		public BaseView(Context context) : base(context) { }

        public virtual void LayoutSubviews() { }

        protected void SetMainViewFrame()
        {
            Frame = new CGRect(0, 0, Resources.DisplayMetrics.WidthPixels, UsableHeight);    
        }

		public void CloseKeyboard()
		{
			if (!(Context is Activity))
			{
				return;
			}

			View view = (Context as Activity).CurrentFocus;

			if (view != null)
			{
				var service = Context.GetSystemService(Context.InputMethodService);
				var manager = service as InputMethodManager;
				manager.HideSoftInputFromWindow(view.WindowToken, 0);
			}
		}

		public DisplayMetrics Metrics
		{
			get { return Context.Resources.DisplayMetrics; }
		}

		public bool IsLandscape
		{
			get { return Metrics.WidthPixels > Metrics.HeightPixels; }
		}

		public bool IsLargeTablet
		{
			get
			{
				var width = Metrics.WidthPixels;
				var height = Metrics.HeightPixels;

				var greater = height;
				var lesser = width;

				if (IsLandscape)
				{
					greater = width;
					lesser = height;
				}

				if (Density > 2.5f)
				{
					// If density is too large, it'll be a phone
					return false;
				}

				return greater > 1920 && lesser > 1080;
			}
		}

		public void Elevate()
		{
			if (this.IsLollipopOrHigher())
			{
				Elevation = 5.0f;
			}
			else
			{
				int width = (int)(1 * Density);

				if (width > 1)
				{
					width -= 1;
				}

                var color = Color.Argb(90, 0, 0, 0);
				this.SetBorderColor(width, color);
			}
		}

        public void AnimateAlpha(float to, int duration = 200)
        {
            var animator = ObjectAnimator.OfFloat(this, "Alpha", to);
            animator.SetDuration(duration);
            animator.Start();
        }

        public void AnimateX(int to, long duration = 200)
        {
            var animator = ObjectAnimator.OfFloat(this, "x", to);
            animator.SetDuration(duration);
            animator.Start();

            UpdateInternalX(to);
        }

        public void AnimateFrame(int x, int y, int w, int h)
        {
            var animation = new ResizeAnimation(this);

            animation.NewX = x;
            animation.NewY = y;
            animation.NewWidth = w;
            animation.NewHeight = h;

            animation.Duration = 300;

            StartAnimation(animation);
        }

        public void UpdateFrameFromParams()
        {
            var parameters = (RelativeLayout.LayoutParams)LayoutParameters;
            Frame = new CGRect(parameters.LeftMargin, parameters.TopMargin,
                               parameters.Width, parameters.Height);
        }

	}
}
