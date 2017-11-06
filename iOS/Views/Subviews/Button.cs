
using System;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class Button : UIView
    {
        UIImageView image;

        public Button(string url)
        {
            image = new UIImageView(UIImage.FromFile(url));
			image.ClipsToBounds = true;
			image.ContentMode = UIViewContentMode.ScaleAspectFit;
            AddSubview(image);

            BackgroundColor = Colors.CartoNavy;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            nfloat padding = Frame.Height / (nfloat)3.8f;

            image.Frame = new CGRect(padding, padding, Frame.Width - 2 * padding, Frame.Height - 2 * padding);

            Layer.CornerRadius = Frame.Width / 2;

            this.AddRoundShadow();
        }

        UITapGestureRecognizer recognizer;

        public void AddGestureRecognizer(Action action)
        {
            recognizer = new UITapGestureRecognizer(action);
            AddGestureRecognizer(recognizer);   
        }

        public void RemoveGestureRecognizer()
        {
            RemoveGestureRecognizer(recognizer);
        }
    }
}
