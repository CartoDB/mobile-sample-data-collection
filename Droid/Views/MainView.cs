
using System;
using Android.Content;
using Android.Widget;
using Carto.Layers;
using Carto.Ui;

namespace data.collection.Droid
{
    public class MainView : BaseView
    {
        public Banner Banner { get; private set; }

		public TextEntry TitleField { get; private set; }

		public TextEntry DescriptionField { get; private set; }

		public ImageEntry PhotoField { get; private set; }

        public ImageEntry LocationField { get; private set; }

		public MainView(Context context) : base(context)
        {
            Banner = new Banner(context);
            AddView(Banner);

            TitleField = new TextEntry(context, "TITLE");
            AddView(TitleField);

            DescriptionField = new TextEntry(context, "DESCRIPTION");
            AddView(DescriptionField);

            PhotoField = new ImageEntry(context, "TAKE PHOTO", Resource.Drawable.icon_camera);
            AddView(PhotoField);

            LocationField = new ImageEntry(context, "ADD LOCATION", Resource.Drawable.icon_add_location);
			AddView(LocationField);

            SetMainViewFrame();
        }

        public override void LayoutSubviews()
        {
            int x = 0;
            int y = 0;
            int h = (int)(45 * Density);
            int w = Frame.W;

            int padding = (int)(15 * Density);

            Banner.Frame = new CGRect(x, y, w, h);

			x = padding;
			y = padding;
            w = Frame.W - 2 * padding;
            h = (int)(60 * Density);

			TitleField.Frame = new CGRect(x, y, w, h);

			y += h + padding;
            h = (int)(150 * Density);

			DescriptionField.Frame = new CGRect(x, y, w, h);

			y += h + padding;
            w = (Frame.W - 3 * padding) / 2;

			PhotoField.Frame = new CGRect(x, y, w, h);

            x += w + padding;

            LocationField.Frame = new CGRect(x, y, w, h);
        }

    }
}
