using System;
using Android.Content;
using Android.Graphics;
using Android.Widget;

namespace data.collection.Droid
{
    public class AttachmentImage : BaseView
    {
        ImageView photoView;
        ProgressBar spinner;
        ImageView closeButton;

        public AttachmentImage(Context context) : base(context)
        {
            this.SetBackground(Colors.DarkTransparentGray);

            photoView = new ImageView(context);
            AddView(photoView);

            spinner = new ProgressBar(context);
            AddView(spinner);

            closeButton = new ImageView(context);
            closeButton.SetImageResource(Resource.Drawable.icon_close_white);
            closeButton.SetScaleType(ImageView.ScaleType.CenterCrop);
            AddView(closeButton);

            photoView.Click += delegate
            {
                if (Frame.IsEqual(original) || original.IsEqual(CGRect.Empty))
                {
                    Expand();    
                }
                else
                {
                    Collapse();
                }
            };

            closeButton.Visibility = Android.Views.ViewStates.Gone;
        }

        int Padding { get { return (int)(5 * Density); } }

        public override void LayoutSubviews()
        {
            int x = Padding;
            int y = Padding;
            int w = Frame.W - 2 * Padding;
            int h = Frame.H - 2 * Padding;

            photoView.SetFrame(x, y, w, h);

            w = Frame.W / 4;
            h = w;
            x = Frame.W / 2 - w / 2;
            y = Frame.H / 2 - h / 2;

            spinner.SetFrame(x, y, w, h);

            w = (int)(30 * Density);
            h = w;
            x = Frame.W - (w + Padding);
            y = Padding;

            closeButton.SetFrame(x, y, w, h);
        }

        public void SetImage(Bitmap bitmap)
        {
            photoView.SetImageBitmap(bitmap);
            spinner.Visibility = Android.Views.ViewStates.Gone;
        }

        CGRect original = CGRect.Empty;

        void Expand()
        {
            original = Frame;

            int width = (Parent as BaseView).Frame.W - 2 * Padding;
            AnimateFrame(Padding, Padding, width, Frame.H * 2);
            closeButton.Visibility = Android.Views.ViewStates.Visible;
        }

        public void Collapse(bool animated = true)
        {
            if (animated)
            {
                AnimateFrame(original.X, original.Y, original.W, original.H);    
            }
            else 
            {
                Frame = original;
            }

            closeButton.Visibility = Android.Views.ViewStates.Gone;
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
