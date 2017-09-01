
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Carto.Layers;
using Carto.Ui;

namespace data.collection.Droid
{
    public class LocationChoiceView : BannerView
    {
		public MapView MapView { get; private set; }

        public ActionButton Done { get; private set; }

		public LocationChoiceView(Context context) : base(context)
        {
			MapView = new MapView(context);
			AddView(MapView);

            Done = new ActionButton(context, Resource.Drawable.icon_done);
            AddView(Done);

            SetMainViewFrame();

			var layer = new CartoOnlineVectorTileLayer(CartoBaseMapStyle.CartoBasemapStyleVoyager);
			MapView.Layers.Add(layer);

            Banner.BringToFront();

            Done.Hide();
		}

		public override void LayoutSubviews()
		{
            base.LayoutSubviews();

			MapView.SetFrame(0, 0, Frame.W, Frame.H);

            int pad = 15;

			int w = 55;
			int h = w;
            int x = Frame.W - (w + pad);
            int y = Frame.H - (h + pad);

			Done.Frame = new CGRect(x, y, w, h);

			int legendPadding = 5;

			w = 220;
			h = 100;
			x = legendPadding;
            y = Frame.H - (h + legendPadding);

			//Legend.Frame = new CGRect(x, y, w, h);
		}

	}
}
