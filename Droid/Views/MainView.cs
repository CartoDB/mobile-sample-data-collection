
using System;
using Android.Content;
using Carto.Layers;
using Carto.Ui;

namespace data.collection.Droid
{
    public class MainView : BannerView
    {
		public MapView MapView { get; private set; }

        public Legend Legend { get; private set; }

        public SlideInPopup Popup { get; private set; }

        public PopupContent Content { get; private set; }

		public MainView(Context context) : base(context)
        {
			MapView = new MapView(context);
			AddView(MapView);

            Legend = new Legend(context);
            AddView(Legend);

            // Just use close icon for both images,
            // as back icon is not used in this application
            int close = Resource.Drawable.icon_close;
            Popup = new SlideInPopup(context, close, close);
            AddView(Popup);

            SetMainViewFrame();

			var layer = new CartoOnlineVectorTileLayer(CartoBaseMapStyle.CartoBasemapStyleVoyager);
			MapView.Layers.Add(layer);

            Banner.BringToFront();
            Popup.BringToFront();

            Content = new PopupContent(context);
            Popup.SetPopupContent(Content);
            Popup.Header.Text = "SUBMIT A NEW LOCATION";
		}

		public override void LayoutSubviews()
		{
            base.LayoutSubviews();

			MapView.SetFrame(0, 0, Frame.W, Frame.H);

            int legendPadding = (int)(5 * Density);

            int w = (int)(220 * Density);
            int h = (int)(100 * Density);
            int x = Frame.W - (w + legendPadding);
            int y = legendPadding;

			Legend.Frame = new CGRect(x, y, w, h);

            x = 0;
            y = 0;
            w = Frame.W;
            h = Frame.H;

            Popup.Frame = new CGRect(x, y, w, h);
		}

    }
}
