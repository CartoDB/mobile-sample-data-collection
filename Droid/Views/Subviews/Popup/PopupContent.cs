
using System;
using Android.Content;
using Android.Graphics;
using Android.Views.InputMethods;

namespace data.collection.Droid
{
    public class PopupContent : BaseView
    {
        public TextEntry TitleField { get; private set; }

        public TextEntry DescriptionField { get; private set; }

        public ActionButton Done { get; private set; }

        public ImageEntry CameraField { get; private set; }

        public PopupContent(Context context) : base(context)
        {
            TitleField = new TextEntry(context, "TITLE", true);
            TitleField.ImeOptions = ImeAction.Next;
            AddView(TitleField);

            DescriptionField = new TextEntry(context, "DESCRIPTION", true);
            AddView(DescriptionField);

            CameraField = new ImageEntry(context, "TAKE PHOTO", Resource.Drawable.icon_camera);
            AddView(CameraField);

            Done = new ActionButton(context, Resource.Drawable.icon_done);
            Done.SetBackground(Colors.AppleBlue);
            AddView(Done);

            Click += (sender, e) =>
            {
                // Click event just so it wouldn't close on click
            };
        }

        public override void LayoutSubviews()
        {
            int pad = (int)(15 * Density);

            int w = (int)(55 * Density);
            int h = w;
            int x = Frame.W - (w + pad);
            int y = Frame.H - (h + pad);

            y = pad;

            Done.Frame = new CGRect(x, y, w, h);

            w = Frame.W - (w + 3 * pad);
            h = (int)(60 * Density);
            x = pad;
            y = 0;

            TitleField.Frame = new CGRect(x, y, w, h);

            y += h;

            DescriptionField.Frame = new CGRect(x, y, w, h);

            y += h + pad / 2;

            h = (int)(1.3 * h);
            w = (int)(1.1 * h);

            CameraField.Frame = new CGRect(x, y, w, h);
        }
    }
}
