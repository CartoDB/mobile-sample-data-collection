using System;
using Android.Content;
using Android.Graphics;
using Android.Widget;

namespace data.collection.Droid
{
    public class AttachmentImage : BaseView
    {
        ImageView imageView;
        ProgressBar spinner;

        public AttachmentImage(Context context) : base(context)
        {
            this.SetBackground(Colors.DarkTransparentGray);

            imageView = new ImageView(context);
            AddView(imageView);

            spinner = new ProgressBar(context);
            AddView(spinner);
        }

        public override void LayoutSubviews()
        {
            int padding = (int)(5 * Density);

            int x = padding;
            int y = padding;
            int w = Frame.W - 2 * padding;
            int h = Frame.H - 2 * padding;

            imageView.SetFrame(x, y, w, h);

            w = Frame.W / 4;
            h = w;
            x = Frame.W / 2 - w / 2;
            y = Frame.H / 2 - h / 2;

            spinner.SetFrame(x, y, w, h);
        }

        public void SetImage(Bitmap bitmap)
        {
            imageView.SetImageBitmap(bitmap);
            spinner.Visibility = Android.Views.ViewStates.Gone;
        }

        public void Show()
        {
            AnimateAlpha(1);
            spinner.Visibility = Android.Views.ViewStates.Visible;
        }

        public void Hide()
        {
            AnimateAlpha(0);
        }
    }
}
