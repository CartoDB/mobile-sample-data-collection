
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

            TitleField.Field.SetCornerRadius((int)(5 * Density));
            DescriptionField.Field.SetCornerRadius((int)(5 * Density));

            Click += (sender, e) =>
            {
                // Click event just so it wouldn't close on click
            };
        }

        public override void LayoutSubviews()
        {
            int pad = (int)(15 * Density);
            int doneSize = (int)(55 * Density);

            int w = Frame.W - (doneSize + 3 * pad);
            int h = (int)(60 * Density);
            int x = pad;
            int y = 0;

            TitleField.Frame = new CGRect(x, y, w, h);

            y += h;
            h += h;

            DescriptionField.Frame = new CGRect(x, y, w, h);

            y += h + pad / 2;

            x += (int)(5 * Density);
            h = (int)(71 * Density);
            w = (int)(1.1 * h);

            CameraField.Frame = new CGRect(x, y, w, h);

            y += (CameraField.Frame.H - doneSize) / 2;
            x = TitleField.Frame.Right - doneSize;
            w = doneSize;
            h = w;

            Done.Frame = new CGRect(x, y, w, h);
        }

        public void Clear()
        {
            TitleField.Text = "";
            DescriptionField.Text = "";

            CameraField.Clear();

        }
    }
}
