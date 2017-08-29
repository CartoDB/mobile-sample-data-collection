
using System;
using Android.Content;
using Android.Widget;
using Carto.Layers;
using Carto.Ui;

namespace data.collection.Droid
{
    public class MainView : BaseView
    {
        public MapView MapView { get; private set; }

        public Banner Banner { get; private set; }

		public TextEntry TitleField { get; private set; }

		public TextEntry DescriptionField { get; private set; }

		public ImageEntry ImageField { get; private set; }

        public SubmitButton Submit { get; private set; }

		public MainView(Context context) : base(context)
        {
            MapView = new MapView(context);
            AddView(MapView);

            Banner = new Banner(context);
            AddView(Banner);

            TitleField = new TextEntry(context, "TITLE");
            AddView(TitleField);

            DescriptionField = new TextEntry(context, "DESCRIPTION");
            AddView(DescriptionField);

            ImageField = new ImageEntry(context, "TAKE PHOTO", Resource.Drawable.icon_camera);
            AddView(ImageField);

            Submit = new SubmitButton(context);
            AddView(Submit);

            SetMainViewFrame();

            var layer = new CartoOnlineVectorTileLayer(CartoBaseMapStyle.CartoBasemapStyleVoyager);
            MapView.Layers.Add(layer);
        }

        public override void LayoutSubviews()
        {
            MapView.SetFrame(0, 0, Frame.W, Frame.H);

            int x = 0;
            int y = 0;
            int h = (int)(45 * Density);
            int w = Frame.W;

            int padding = (int)(15 * Density);

            Banner.Frame = new CGRect(x, y, w, h);

			x = padding;
			y = Banner.Frame.Bottom - padding;
            w = Frame.W - 2 * padding;
            h = (int)(60 * Density);

			TitleField.Frame = new CGRect(x, y, w, h);

			y += h + padding;
            h = (int)(150 * Density);

			DescriptionField.Frame = new CGRect(x, y, w, h);

			y += h + padding;

			ImageField.Frame = new CGRect(x, y, w, h);

			y += h + padding;
            h = (int)(50 * Density);

			Submit.Frame = new CGRect(x, y, w, h);
        }

    }
}
