
using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace data.collection.iOS
{
    public class BaseEntry : UIView
    {
		protected UILabel label;

        public BaseEntry(string title, bool isRequired)
        {
			label = new UILabel();
            label.TextColor = Colors.CartoNavy;
			
			label.Font = UIFont.FromName("HelveticaNeue", 11);
			AddSubview(label);

            if (isRequired)
            {
                var attributed = new NSMutableAttributedString(title + " *");

                var attribute = GetColorAttribute(Colors.CartoNavy);
                var range = new NSRange(0, title.Length);                         
                attributed.SetAttributes(attribute, range);

                attribute = GetColorAttribute(Colors.CartoRed);
                range = new NSRange(title.Length, 2);
                attributed.SetAttributes(attribute, range);

                attribute.Font = UIFont.FromName("HelveticaNeue-Bold", 13);
                attributed.SetAttributes(attribute, range);

                label.AttributedText = attributed;
            }
            else 
            {
                label.Text = title;        
            }
		}
		
        UIStringAttributes GetColorAttribute(UIColor color)
        {
            return new UIStringAttributes { ForegroundColor = color };
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
