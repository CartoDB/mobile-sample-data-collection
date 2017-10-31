
using System;
using Android.Content;
using Android.Widget;
using Carto.Core;
using Carto.Layers;
using Carto.Projections;
using Carto.Ui;

namespace data.collection.Droid
{
    public class _deprecatedView : BannerView
    {
		public TextEntry TitleField { get; private set; }

		public TextEntry DescriptionField { get; private set; }

		public ImageEntry CameraField { get; private set; }

        public ImageEntry LocationField { get; private set; }

        public _deprecatedView(Context context) : base(context)
        {
            TitleField = new TextEntry(context, "TITLE");
            AddView(TitleField);

            DescriptionField = new TextEntry(context, "DESCRIPTION");
            AddView(DescriptionField);

            CameraField = new ImageEntry(context, "TAKE PHOTO", Resource.Drawable.icon_camera);
            AddView(CameraField);

            LocationField = new ImageEntry(context, "ADD LOCATION", Resource.Drawable.icon_add_location);
			AddView(LocationField);

            SetMainViewFrame();

            Banner.BringToFront();
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			int x = padding;
			int y = padding;
            int w = Frame.W - 2 * padding;
            int h = (int)(60 * Density);

			TitleField.Frame = new CGRect(x, y, w, h);

			y += h + padding;
            h = (int)(150 * Density);

			DescriptionField.Frame = new CGRect(x, y, w, h);

			y += h + padding;
            w = (Frame.W - 3 * padding) / 2;

			CameraField.Frame = new CGRect(x, y, w, h);

            x += w + padding;

            LocationField.Frame = new CGRect(x, y, w, h);
        }

		MapView mapView;
		MapView MapView
		{
			get
			{
				if (mapView == null)
				{
                    mapView = new MapView(Context);
				}

				return mapView;
			}
		}

		Projection Projection => MapView.Options.BaseProjection;

		public void AddMapOverlayTo(double longitude, double latitude)
		{
			MapPos position = Projection.FromWgs84(new MapPos(longitude, latitude));
			LocationField.SetMap(MapView, position);
		}

        public bool IsAnyFieldEmpty
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(TitleField.Text))
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(DescriptionField.Text))
                {
                    return true;
                }

                if (CameraField.Photo == null)
                {
                    return true;
                }

                if (!LocationField.IsSet)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
