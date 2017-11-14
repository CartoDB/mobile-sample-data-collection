using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;

namespace data.collection.Droid
{
    public class AttachmentImage : BaseView
    {
        ImageView placeHolder;
        ImageView photoView;
        ProgressBar spinner;
        ImageView closeButton;

        public AttachmentImage(Context context) : base(context)
        {
            this.SetBackground(Colors.DarkTransparentGray);

            placeHolder = new ImageView(context);
            placeHolder.SetImageResource(Resource.Drawable.icon_no_image);
            placeHolder.SetScaleType(ImageView.ScaleType.CenterCrop);
            AddView(placeHolder);

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
                if (Frame.IsEqual(original) || original.IsEmpty)
                {
                    Expand();    
                }
                else
                {
                    Collapse();
                }
            };

            closeButton.Visibility = ViewStates.Gone;
            placeHolder.Visibility = ViewStates.Gone;
        }

        int Padding { get { return (int)(5 * Density); } }

        public override void LayoutSubviews()
        {
            int w = Frame.W / 2;
            int h = w;
            int x = Frame.W / 2 - w / 2;
            int y = Frame.H / 2 - h / 2;

            placeHolder.SetFrame(x, y, w, h);

            x = Padding;
            y = Padding;
            w = Frame.W - 2 * Padding;
            h = Frame.H - 2 * Padding;

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
            photoView.Visibility = ViewStates.Visible;
            placeHolder.Visibility = ViewStates.Gone;
            spinner.Visibility = ViewStates.Gone;
        }

        public void ShowPlaceHolder()
        {
            placeHolder.Visibility = ViewStates.Visible;
            photoView.Visibility = ViewStates.Gone;
            spinner.Visibility = ViewStates.Gone;
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
                if (!original.IsEmpty)
                {
                    Frame = original;
                }
            }

            closeButton.Visibility = Android.Views.ViewStates.Gone;
        }

        public void Show()
        {
            AnimateAlpha(1);
            spinner.Visibility = Android.Views.ViewStates.Visible;
            placeHolder.Visibility = ViewStates.Gone;
        }

        public void Hide()
        {
            AnimateAlpha(0);
        }
    }
}
