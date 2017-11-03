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

            imageView.Click += delegate
            {
                int width = (Parent as BaseView).Frame.W - 2 * Padding;
                AnimateFrame(Padding, Padding, width, (int)(Frame.H * 2));
            };
        }

        int Padding { get { return (int)(5 * Density); } }

        public override void LayoutSubviews()
        {
            
            int x = Padding;
            int y = Padding;
            int w = Frame.W - 2 * Padding;
            int h = Frame.H - 2 * Padding;

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
