
using System;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class BaseEntry : UIView
    {
		protected UILabel label;

        public BaseEntry(string title)
        {
			BackgroundColor = Colors.DarkTransparentGray;

			Layer.BorderWidth = 1;
			Layer.BorderColor = UIColor.LightGray.CGColor;

			label = new UILabel();
            label.TextColor = UIColor.White;
			label.Text = title;
			label.Font = UIFont.FromName("HelveticaNeue", 10);
			AddSubview(label);
		}
		
        protected nfloat padding = 5;

		public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			label.SizeToFit();
			
			nfloat x = padding;
			nfloat y = padding;
			nfloat w = Frame.Width - 2 * padding;
			nfloat h = label.Frame.Height;

			label.Frame = new CGRect(x, y, w, h);
		}
    }
}
