
using System;
using Android.Content;
using Android.Graphics;
using Android.Widget;

namespace data.collection.Droid
{
    public class Legend : BaseView
    {
		LegendRow mycurrentLocation, synced, mySynced, unsynced;

		public Legend(Context context) : base(context)
        {
            this.SetBackground(Colors.DarkTransparentGray);

            mycurrentLocation = new LegendRow(context);
            mycurrentLocation.Indicator.SetBackground(Colors.DarkTransparentAppleBlue);
			mycurrentLocation.Label.Text = "CURRENT LOCATION";
            AddView(mycurrentLocation);

			synced = new LegendRow(context);
			AddView(synced);

			mySynced = new LegendRow(context);
			AddView(mySynced);

			unsynced = new LegendRow(context);
			AddView(unsynced);

            Visibility = Android.Views.ViewStates.Gone;
        }

		public override void LayoutSubviews()
		{
			int x = 0;
			int y = 0;
            int w = Frame.W;
            int h = Frame.H / 4;

			mycurrentLocation.Frame = new CGRect(x, y, w, h);

			y += h;

			synced.Frame = new CGRect(x, y, w, h);

			y += h;

			mySynced.Frame = new CGRect(x, y, w, h);

			y += h;

			unsynced.Frame = new CGRect(x, y, w, h);
		}

		public void Update(Color syncedColor, Color mySyncedColor, Color unsyncedColor)
		{
            Visibility = Android.Views.ViewStates.Visible;

            synced.Indicator.SetBackground(syncedColor);
			synced.Label.Text = "LOCATIONS";

			mySynced.Indicator.SetBackground(mySyncedColor);
			mySynced.Label.Text = "MY LOCATIONS";

            unsynced.Indicator.SetBackground(unsyncedColor);
			unsynced.Label.Text = "UNSYNCED LOCATIONS";

            LayoutSubviews();
		}
    }

	public class LegendRow : BaseView
	{
        public BaseView Indicator { get; private set; }

        public TextView Label { get; private set; }
		int padding = 5;
		int height = 10;

		public LegendRow(Context context) : base(context)
		{
			padding = (int)(5 * Density);
			height = (int)(10 * Density);

            Indicator = new BaseView(context);
            AddView(Indicator);

            Label = new TextView(context);
            Label.SetTextColor(Color.White);
            Label.TextSize = 10f;
            Label.Gravity = Android.Views.GravityFlags.CenterVertical;
            AddView(Label);
		}

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			int w = height;
			int h = w;
			int x = padding;
            int y = Frame.H / 2 - h / 2;

			Indicator.Frame = new CGRect(x, y, w, h);
            Indicator.SetCornerRadius(w / 2);

			x += w + padding;

			y = 0;
            w = Frame.W - (w + 3 * padding);
            h = Frame.H;

            Label.SetFrame(x, y, w, h);
        }
	}
}
