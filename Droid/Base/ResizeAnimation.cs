using System;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;

namespace data.collection.Droid
{
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
            (View as BaseView).UpdateFrameFromParams();
        }

        public override bool WillChangeBounds()
        {
            return true;
        }
    }
}
