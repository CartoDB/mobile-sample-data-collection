
using System;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class SubmitButton : UIView
    {
        public EventHandler<EventArgs> Click;
		
        UIImageView imageView;
		UILabel label;

        public SubmitButton()
        {
            BackgroundColor = Colors.TransparentAppleBlue;

            Layer.BorderWidth = 1;
            Layer.BorderColor = Colors.DarkTransparentAppleBlue.CGColor;

			imageView = new UIImageView();
			imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
			imageView.ClipsToBounds = true;
			imageView.Image = UIImage.FromFile("icon_done.png");
			AddSubview(imageView);

			label = new UILabel();
			label.Font = UIFont.FromName("HelveticaNeue", 12);
			label.TextAlignment = UITextAlignment.Center;
			label.TextColor = UIColor.White;
            label.Text = "SUBMIT";
			AddSubview(label);
        }

		public override void LayoutSubviews()
		{
			nfloat padding = Frame.Height / 4;

			nfloat x = padding;
			nfloat y = padding;
			nfloat w = Frame.Height - 2 * padding;
			nfloat h = w;

			imageView.Frame = new CGRect(x, y, w, h);

			x += w + padding;
			y = 0;
			w = Frame.Width - (2 * x);
			h = Frame.Height;

			label.Frame = new CGRect(x, y, w, h);
		}

        public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesBegan(touches, evt);
            Alpha = 0.5f;
        }

        public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesEnded(touches, evt);
            Alpha = 1.0f;

            Click?.Invoke(this, EventArgs.Empty);
        }

        public override void TouchesCancelled(Foundation.NSSet touches, UIEvent evt)
        {
            base.TouchesCancelled(touches, evt);
            Alpha = 1.0f;
        }
    }
}
