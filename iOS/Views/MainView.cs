using System;
using Carto.Layers;
using Carto.Ui;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class MainView : BaseView
    {
		public MapView MapView { get; private set; }

        public ActionButton Done { get; private set; }

        public MainView()
        {
			MapView = new MapView();
			AddSubview(MapView);

            Done = new ActionButton("icon_done.png");
            Done.BackgroundColor = Colors.DarkTransparentAppleBlue;
            AddSubview(Done);

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

            nfloat legendPadding = 5;
        }

    }
}
