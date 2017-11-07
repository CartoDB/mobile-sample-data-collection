
using System;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class PopupContent : UIView
    {
        public TextEntry TitleField { get; private set; }

        public TextEntry DescriptionField { get; private set; }

        public ActionButton Done { get; private set; }

        public ImageEntry CameraField { get; private set; }

        public PopupContent()
        {
            TitleField = new TextEntry("TITLE");
            AddSubview(TitleField);

            DescriptionField = new TextEntry("DESCRIPTION");
            AddSubview(DescriptionField);

            CameraField = new ImageEntry("TAKE PHOTO", "icon_camera.png");
            AddSubview(CameraField);

            Done = new ActionButton("icon_done.png");
            Done.BackgroundColor = Colors.AppleBlue;
            AddSubview(Done);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            nfloat pad = 15.0f;
            nfloat doneSize = 55.0f;

            nfloat w = Frame.Width - (doneSize + 3 * pad);
            nfloat h = 60.0f;
            nfloat x = pad;
            nfloat y = 0;

            TitleField.Frame = new CGRect(x, y, w, h);

            y += h;
            h += h;

            DescriptionField.Frame = new CGRect(x, y, w, h);

            y += h + pad / 2;

            x += 5;
            h = 71;
            w = 1.1f * h;

            CameraField.Frame = new CGRect(x, y, w, h);

            y += (CameraField.Frame.Height - doneSize) / 2;
            x = TitleField.Frame.Right - doneSize;
            w = doneSize;
            h = w;

            Done.Frame = new CGRect(x, y, w, h);
        }
    }
}
