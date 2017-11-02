
using System;
using Android.Content;
using Android.Views;
using Android.Widget;
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
        public ActionButton Done { get; private set; }
        public ActionButton Cancel { get; private set; }

        public ImageView Crosshair { get; private set; }

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

            Done = new ActionButton(context, Resource.Drawable.icon_done);
            Done.SetBackground(Colors.CartoGreen);
            AddView(Done);

            Cancel = new ActionButton(context, Resource.Drawable.icon_remove);
            Cancel.SetBackground(Colors.LocationRed);
            AddView(Cancel);

            Add = new ActionButton(context, Resource.Drawable.icon_add);
            Add.SetBackground(Colors.AppleBlue);
            AddView(Add);

            Crosshair = new ImageView(Context);
            Crosshair.SetImageResource(Resource.Drawable.icon_crosshair);
            Crosshair.SetScaleType(ImageView.ScaleType.CenterCrop);
            Crosshair.SetAdjustViewBounds(true);
            AddView(Crosshair);

            /* 
             * Frame setting. 
             * Everything should be initialized before this point 
             */
            SetMainViewFrame();

            var layer = new CartoOnlineVectorTileLayer(CartoBaseMapStyle.CartoBasemapStyleVoyager);
            MapView.Layers.Add(layer);

            Banner.BringToFront();
            Popup.BringToFront();

            Crosshair.Visibility = ViewStates.Gone;

            Popup.SetPopupContent(Content);
		}

        int doneVisiblePosition, doneHiddenPosition = -1;

		public override void LayoutSubviews()
		{
            base.LayoutSubviews();

			MapView.SetFrame(0, 0, Frame.W, Frame.H);

            Popup.Frame = new CGRect(0, 0, Frame.W, Frame.H);

            int w = (int)(58 * Density);
            int h = w;
            int x = Frame.W - (w + padding);
            int y = Frame.H - (h + padding);

            Add.Frame = new CGRect(x, y, w, h);
            Done.Frame = new CGRect(x, y, w, h);
            Cancel.Frame = new CGRect(x, y, w, h);

            doneVisiblePosition = Done.Frame.X - (Done.Frame.W + padding);
            doneHiddenPosition = Done.Frame.X;
            x = MapView.LayoutParameters.Width / 2 - w / 2;
            y = MapView.LayoutParameters.Height / 2 - h / 2;

            Crosshair.SetFrame(x, y, w, h);
		}

        public void SetCrosshairMode()
        {
            Crosshair.Visibility = ViewStates.Visible;

            Add.Hide();

            Done.AnimateX(doneVisiblePosition);
        }

        public void CancelCrosshairMode()
        {
            Crosshair.Visibility = ViewStates.Gone;

            Add.Show();

            Done.AnimateX(doneHiddenPosition);
        }
    }
}
