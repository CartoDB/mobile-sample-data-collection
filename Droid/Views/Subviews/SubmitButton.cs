
using System;
using Android.Content;
using Android.Graphics;
using Android.Widget;

namespace data.collection.Droid
{
    public class SubmitButton : BaseView
    {
        ImageView imageView;
        TextView label;

        public SubmitButton(Context context) : base(context)
		{
            this.SetBackground(Colors.TransparentAppleBlue);

			imageView = new ImageView(context);
			imageView.SetImageResource(Resource.Drawable.icon_done);
			imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
			AddView(imageView);

			label = new TextView(context);
			label.Gravity = Android.Views.GravityFlags.Center;
			label.TextSize = 12.0f;
			label.SetTextColor(Color.White);
            label.Text = "SUBMIT";
			AddView(label);
		}

        public override void LayoutSubviews()
        {
            int padding = Frame.H / 4;

			int x = padding;
			int y = padding;
            int w = Frame.H - 2 * padding;
			int h = w;

            imageView.SetFrame(x, y, w, h);

			x += w + padding;
			y = 0;
            w = Frame.W - (2 * x);
            h = Frame.H;

			label.SetFrame(x, y, w, h);
        }
    }
}
