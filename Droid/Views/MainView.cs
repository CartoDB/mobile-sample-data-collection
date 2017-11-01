
using System;
using Android.Content;
using Carto.Layers;
using Carto.Ui;

namespace data.collection.Droid
{
    public class MainView : BannerView
    {
		public MapView MapView { get; private set; }

        public SlideInPopup Popup { get; private set; }

        public PopupContent Content { get; private set; }

        public bool IsAnyRequiredFieldEmpty 
        {
            get { return Content.TitleField.IsEmpty || Content.DescriptionField.IsEmpty; }
        }

        public MainView(Context context) : base(context)
        {
			MapView = new MapView(context);
			AddView(MapView);

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

            Popup.Frame = new CGRect(0, 0, Frame.W, Frame.H);
		}

    }
}
