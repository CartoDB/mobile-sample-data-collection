
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

        public ActionButton Add { get; private set; }

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

            Content = new PopupContent(context);
            Popup.SetPopupContent(Content);
            Popup.Header.Text = "SUBMIT A NEW LOCATION";

            Add = new ActionButton(context, Resource.Drawable.icon_add);
            Add.SetBackground(Colors.AppleBlue);
            AddView(Add);

            /* 
             * Frame setting. 
             * Everything should be initialized before this point 
             */
            SetMainViewFrame();

            var layer = new CartoOnlineVectorTileLayer(CartoBaseMapStyle.CartoBasemapStyleVoyager);
            MapView.Layers.Add(layer);

            Banner.BringToFront();
            Popup.BringToFront();
		}

		public override void LayoutSubviews()
		{
            base.LayoutSubviews();

			MapView.SetFrame(0, 0, Frame.W, Frame.H);

            Popup.Frame = new CGRect(0, 0, Frame.W, Frame.H);

            int padding = (int)(15 * Density);

            int w = (int)(55 * Density);
            int h = w;
            int x = Frame.W - (w + padding);
            int y = Frame.H - (h + padding);

            Add.Frame = new CGRect(x, y, w, h);
		}

    }
}
