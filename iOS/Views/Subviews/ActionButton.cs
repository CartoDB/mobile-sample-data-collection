
using System;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class ActionButton : UIView
    {
        public EventHandler<EventArgs> Click;

        protected UIImageView imageView;
        protected UIImage image;

        public ActionButton(string imageUrl)
        {
            BackgroundColor = UIColor.White;

            imageView = new UIImageView();
            imageView.ClipsToBounds = true;
            imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
            AddSubview(imageView);

            image = UIImage.FromFile(imageUrl);
            imageView.Image = image;

            imageView.UserInteractionEnabled = false;
            UserInteractionEnabled = true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            Layer.CornerRadius = Frame.Width / 2;

            var padding = Frame.Height / 3.5f;
            imageView.Frame = new CGRect(padding, padding, Frame.Width - 2 * padding, Frame.Height - 2 * padding);

            this.AddRoundShadow();
        }

        public bool IsEnabled { get; private set; } = true;

        public void Enable()
        {
            IsEnabled = true;
            Alpha = 1.0f;
        }

        public void Disable()
        {
            IsEnabled = false;
            Alpha = 0.5f;
        }

        public override void TouchesBegan(Foundation.NSSet touches, UIEvent evt)
        {
            Alpha = 0.5f;
        }

        public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
        {
            if (!IsEnabled)
            {
                return;
            }

            Click?.Invoke(this, EventArgs.Empty);

            Alpha = 1.0f;
        }

        public override void TouchesCancelled(Foundation.NSSet touches, UIEvent evt)
        {
			if (!IsEnabled)
			{
				return;
			}

			Alpha = 1.0f;
        }
    }
}
