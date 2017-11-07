
using System;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class AttachmentImage : UIView
    {
        UIImageView photoView;
        UIActivityIndicatorView spinner;
        UIImageView closeButton;

        public AttachmentImage()
        {
            BackgroundColor = Colors.DarkTransparentGray;
            ClipsToBounds = true;

            photoView = new UIImageView();
            photoView.ClipsToBounds = true;
            AddSubview(photoView);

            spinner = new UIActivityIndicatorView();
            AddSubview(spinner);

            closeButton = new UIImageView();
            closeButton.Image = UIImage.FromFile("icon_close_white.png");
            AddSubview(closeButton);

            closeButton.Hidden = true;

            Alpha = 0.0f;
        }

        nfloat Padding { get { return 5.0f; } }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            nfloat x = Padding;
            nfloat y = Padding;
            nfloat w = Frame.Width - 2 * Padding;
            nfloat h = Frame.Height - 2 * Padding;

            photoView.Frame = new CGRect(x, y, w, h);

            w = Frame.Width / 4;
            h = w;
            x = Frame.Width / 2 - w / 2;
            y = Frame.Height / 2 - h / 2;

            spinner.Frame = new CGRect(x, y, w, h);

            w = 30.0f;
            h = w;
            x = Frame.Width - (w + Padding);
            y = Padding;

            closeButton.Frame = new CGRect(x, y, w, h);
        }

        CGRect original = CGRect.Empty;

        public override void TouchesEnded(Foundation.NSSet touches, UIEvent evt)
        {
            if (Frame.Equals(original) || original.IsEmpty)
            {
                Expand();
            }
            else
            {
                Collapse();
            }
        }

        public bool IsAnimating { get; private set; }

        void Expand()
        {
            IsAnimating = true;
            original = Frame;

            nfloat width = (Superview as UIView).Frame.Width - 2 * Padding;
            Animate(0.2, delegate
            {
                Frame = new CGRect(Padding, Device.TrueY0 + Padding, width, Frame.Height * 2);
            }, delegate 
            {
                IsAnimating = false;
            });

            closeButton.Hidden = false;
        }

        public void Collapse(bool animated = true)
        {
            if (animated)
            {
                IsAnimating = true;
                Animate(0.2, delegate
                {
                    Frame = original;
                }, delegate
                {
                    IsAnimating = false;
                });
            }
            else
            {
                if (!original.IsEmpty)
                {
                    Frame = original;
                }
            }

            closeButton.Hidden = true;
        }

        public void SetImage(UIImage image)
        {
            photoView.Image = image;
            spinner.StopAnimating();
        }

        public void Show()
        {
            AnimateAlpha(1.0f);
            spinner.StartAnimating();
        }

        public void Hide()
        {
            AnimateAlpha(0.0f);
        }

        void AnimateAlpha(float to)
        {
            Animate(0.2, delegate
            {
                Alpha = to;
            });
        }
    }
}
