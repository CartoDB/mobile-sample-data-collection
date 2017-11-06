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
            SetInternalFrame(x, y, w, h);

            var animation = new ResizeAnimation(this);

            animation.NewX = x;
            animation.NewY = y;
            animation.NewWidth = w;
            animation.NewHeight = h;

            animation.Duration = 300;

            StartAnimation(animation);
        }
	}

    public class ResizeAnimation : Animation
    {
        public int NewX { get; set; }
        public int NewY { get; set; }
        public int NewWidth { get; set; }
        public int NewHeight { get; set; }

        public View View { get; set; }

        public RelativeLayout.LayoutParams Parameters
        {
            get { return (RelativeLayout.LayoutParams)View.LayoutParameters; }
        }

        public override long Duration
        {
            get { return base.Duration; }
            set { base.Duration = value; }
        }

        int startX, startY, startW, startH;

        public ResizeAnimation(View view)
        {
            View = view;
            Interpolator = new LinearInterpolator();
            startX = Parameters.LeftMargin;
            startY = Parameters.TopMargin;
            startW = Parameters.Width;
            startH = Parameters.Height;
        }

        public override void Initialize(int width, int height, int parentWidth, int parentHeight)
        {
            base.Initialize(width, height, parentWidth, parentHeight);
        }

        protected override void ApplyTransformation(float interpolatedTime, Transformation t)
        {
            Parameters.LeftMargin = startX + (int)((NewX - startX) * interpolatedTime);
            Parameters.TopMargin = startY + (int)((NewY - startY) * interpolatedTime);
            Parameters.Width = startW + (int)((NewWidth - startW) * interpolatedTime);
            Parameters.Height = startH + (int)((NewHeight - startH) * interpolatedTime);

            View.RequestLayout();
            (View as BaseView).LayoutSubviews();
        }

        public override bool WillChangeBounds()
        {
            return true;
        }
    }
}
