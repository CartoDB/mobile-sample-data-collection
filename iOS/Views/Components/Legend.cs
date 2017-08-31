
using System;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class Legend : UIView
    {
        LegendRow mycurrentLocation, synced, mySynced, unsynced;

        public Legend()
        {
            BackgroundColor = Colors.DarkTransparentGray;

			mycurrentLocation = new LegendRow();
            mycurrentLocation.Indicator.BackgroundColor = Colors.DarkTransparentAppleBlue;
            mycurrentLocation.Label.Text = "CURRENT LOCATION";
			AddSubview(mycurrentLocation);

            synced = new LegendRow();
            AddSubview(synced);

			mySynced = new LegendRow();
			AddSubview(mySynced);

			unsynced = new LegendRow();
			AddSubview(unsynced);

            Hidden = true;
        }

        public override void LayoutSubviews()
        {
            nfloat x = 0;
            nfloat y = 0;
            nfloat w = Frame.Width;
            nfloat h = Frame.Height / 4;

            mycurrentLocation.Frame = new CGRect(x, y, w, h);

            y += h;

            synced.Frame = new CGRect(x, y, w, h);

            y += h;

			mySynced.Frame = new CGRect(x, y, w, h);

			y += h;

			unsynced.Frame = new CGRect(x, y, w, h);
        }

        public void Update(UIColor syncedColor, UIColor mySyncedColor, UIColor unsyncedColor)
        {
            Hidden = false;

            synced.Indicator.BackgroundColor = syncedColor;
            synced.Label.Text = "LOCATIONS";

            mySynced.Indicator.BackgroundColor = mySyncedColor;
            mySynced.Label.Text = "MY LOCATIONS";

            unsynced.Indicator.BackgroundColor = unsyncedColor;
            unsynced.Label.Text = "UNSYNCED LOCATIONS";
        }
    }

    public class LegendRow : UIView
    {
        public UIView Indicator { get; private set; }

        public UILabel Label { get; private set; }

        public LegendRow()
        {
            Indicator = new UIView();
            Indicator.Layer.BorderWidth = 0.5f;
            Indicator.Layer.BorderColor = UIColor.White.CGColor;

            AddSubview(Indicator);

            Label = new UILabel();
            Label.Font = UIFont.FromName("HelveticaNeue", 10);
            Label.TextColor = UIColor.White;
            AddSubview(Label);
        }

        nfloat padding = 5;
        nfloat height = 10;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            nfloat w = height;
            nfloat h = w;
			nfloat x = padding;
			nfloat y = Frame.Height / 2 - h / 2;

            Indicator.Frame = new CGRect(x, y, w, h);
            Indicator.Layer.CornerRadius = w / 2;

            x += w + padding;

            y = 0;
            w = Frame.Width - (w + 3 * padding);
            h = Frame.Height;

            Label.Frame = new CGRect(x, y, w, h);
        }
    }
}
