using System;
using Android.Content;
using Android.Graphics;
using Android.Widget;

namespace data.collection.Droid
{
    public class AttachmentImage : BaseView
    {
        ImageView imageView;

        public AttachmentImage(Context context) : base(context)
        {
            this.SetBackground(Colors.DarkTransparentGray);

            imageView = new ImageView(context);
            AddView(imageView);
        }

        public override void LayoutSubviews()
        {
            int padding = (int)(5 * Density);

            imageView.SetFrame(padding, padding, Frame.W - 2 * padding, Frame.H - 2 * padding);
        }

        public void SetImage(Bitmap bitmap)
        {
            imageView.SetImageBitmap(bitmap);
        }

        public void Show()
        {
            AnimateAlpha(1);
        }

        public void Hide()
        {
            AnimateAlpha(0);
        }
    }
}
