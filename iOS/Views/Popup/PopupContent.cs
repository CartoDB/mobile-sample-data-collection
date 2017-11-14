
using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace data.collection.iOS
{
    public class PopupContent : UIView, IUITextViewDelegate
    {
        public EventHandler<EventArgs> BeganEditing;

        public TextEntry TitleField { get; private set; }

        public TextEntry DescriptionField { get; private set; }

        public ActionButton Done { get; private set; }

        public ImageEntry CameraField { get; private set; }

        public nfloat TitleFieldBottom
        {
            get { return GetBottomOf(TitleField); }
        }

        public nfloat DescriptionFieldBottom
        {
            get { return GetBottomOf(DescriptionField); }
        }

        nfloat GetBottomOf(TextEntry field)
        {
            nfloat padding = 5;
            nfloat parent = Superview.Frame.Y + Frame.Y;
            return parent + field.Frame.Y + field.Field.Frame.Bottom + padding;
        }

        public PopupContent()
        {
            TitleField = new TextEntry("TITLE", true);
            TitleField.Field.ReturnKeyType = UIReturnKeyType.Next;
            TitleField.Field.Delegate = this;
            AddSubview(TitleField);

            DescriptionField = new TextEntry("DESCRIPTION", true);
            DescriptionField.Field.ReturnKeyType = UIReturnKeyType.Done;
            DescriptionField.Field.Delegate = this;
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

        [Export("textView:shouldChangeTextInRange:replacementText:")]
        public bool ShouldChangeText(UITextView textView, NSRange range, string text)
        {
            if (text.Equals(Environment.NewLine))
            {
                if (textView == TitleField.Field)
                {
                    DescriptionField.Field.BecomeFirstResponder();
                }
                else
                {
                    this.EndEditing(true);
                }
            }

            return true;
        }

        [Export("textViewShouldBeginEditing:")]
        public bool ShouldBeginEditing(UITextView textView)
        {
            var parent = (TextEntry)textView.Superview;
            BeganEditing?.Invoke(parent, EventArgs.Empty);

            return true;
        }

        public void Clear()
        {
            TitleField.Field.Text = "";
            DescriptionField.Field.Text = "";

            CameraField.Photo.Image = null;
        }
    }
}
