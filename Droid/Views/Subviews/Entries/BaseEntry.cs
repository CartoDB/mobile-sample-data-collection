
using System;
using Android.Content;
using Android.Graphics;
using Android.Widget;

namespace data.collection.Droid
{
    public class BaseEntry : BaseView
    {
        protected TextView label;

        public BaseEntry(Context context, string title) : base(context)
        {
            this.SetBackground(Colors.DarkTransparentGray);

            label = new TextView(context);
            label.Text = title;
            label.SetTextColor(Color.White);
            label.TextSize = 10;
            AddView(label);
        }

        protected int padding
        {
            get { return (int)(5 * Density); }
        }

        public override void LayoutSubviews()
        {
            label.Measure(0, 0);

			int x = padding;
			int y = padding;
            int w = Frame.W - 2 * padding;
            int h = label.MeasuredHeight;

            label.SetFrame(x, y, w, h);
        }
    }
}
