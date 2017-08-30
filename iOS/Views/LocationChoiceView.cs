using System;
using Carto.Layers;
using Carto.Ui;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class LocationChoiceView : BaseView
    {
		public MapView MapView { get; private set; }

        public ActionButton Done { get; private set; }

        public Legend Legend { get; private set; }

		public LocationChoiceView()
        {
			MapView = new MapView();
			AddSubview(MapView);

            Done = new ActionButton("icon_done.png");
            Done.BackgroundColor = Colors.DarkTransparentAppleBlue;
            AddSubview(Done);

            Legend = new Legend();
            AddSubview(Legend);

            var layer = new CartoOnlineVectorTileLayer(CartoBaseMapStyle.CartoBasemapStyleVoyager);
			MapView.Layers.Add(layer);

            Done.Hidden = true;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            MapView.Frame = Bounds;

            nfloat padding = 15;

            nfloat w = 55;
            nfloat h = w;
            nfloat x = Frame.Width - (w + padding);
            nfloat y = Frame.Height - (h + padding);

            Done.Frame = new CGRect(x, y, w, h);

            w = 200;
            h = 80;
            x = padding;
            y = Frame.Height - (h + padding);

            Legend.Frame = new CGRect(x, y, w, h);
        }

    }
}
