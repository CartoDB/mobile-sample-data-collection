
using System;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using Carto.Core;
using Carto.Ui;
using Carto.Layers;

namespace data.collection.Droid
{
    public class ImageEntry : BaseEntry
	{
        ImageView imageView;
        BaseView imageBackground;

        ImageView photoView;

        Bitmap bitmap;
        public Bitmap Photo
        {
            get { return bitmap; }
            set 
            {
                bitmap = value;
                photoView.SetImageBitmap(bitmap);
            }
        }

        public void Clear()
        {
            photoView.SetImageResource(0);
        }

        public string ImageName { get; set; }

        public ImageEntry(Context context, string title, int resource) : base(context, title)
		{
            this.SetBackground(Color.White);
            this.SetBorderColor((int)(1 * Density), Colors.DarkTransparentGray);
            this.SetCornerRadius((int)(5 * Density));

            photoView = new ImageView(context);
            photoView.SetScaleType(ImageView.ScaleType.CenterCrop);
            AddView(photoView);

            imageBackground = new BaseView(context);
            imageBackground.SetBackground(Colors.DarkTransparentGray);
            AddView(imageBackground);

			imageView = new ImageView(context);
            imageView.SetImageResource(resource);
			imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
			AddView(imageView);

            BringChildToFront(label);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            int w = (int)(Frame.H / 3.2);
			int h = w;
            int x = Frame.W / 2 - w / 2;
            int y = Frame.H / 2 - h / 2 + padding;

            imageView.SetFrame(x, y, w, h);

            w = (int)(Frame.H / 1.8);
            h = w;
            x = Frame.W / 2 - w / 2;
            y = Frame.H / 2 - h / 2 + padding;

            imageBackground.Frame = new CGRect(x, y, w, h);
            imageBackground.SetCornerRadius(w / 2);

            photoView.SetFrame(0, 0, Frame.W, Frame.H);
        }

    }
}
