using System;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{

    public class BaseView : UIView
    {
        public Banner Banner { get; private set; }

        public BaseView()
        {
            Banner = new Banner();
            AddSubview(Banner);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			nfloat x = 0;
			nfloat y = Device.TrueY0;
			nfloat w = Frame.Width;
			nfloat h = 45;

			Banner.Frame = new CGRect(x, y, w, h);
		}
    }
}
