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
        public ActionButton AddLocation { get; private set; }
        public ActionButton Cancel { get; private set; }

        public SlideInPopup Popup { get; private set; }

        public UIImageView Crosshair { get; private set; }

        public MainView()
        {
			MapView = new MapView();
			AddSubview(MapView);

            Done = new ActionButton("icon_done.png");
            Done.BackgroundColor = Colors.CartoGreen;
            AddSubview(Done);

            Cancel = new ActionButton("icon_remove.png");
            Cancel.BackgroundColor = Colors.CartoRed;
            AddSubview(Cancel);

            AddLocation = new ActionButton("icon_add.png");
            AddLocation.BackgroundColor = Colors.AppleBlue;
            AddSubview(AddLocation);

            Popup = new SlideInPopup();
            AddSubview(Popup);

            Crosshair = new UIImageView();
            Crosshair.Image = UIImage.FromFile("icon_crosshair.png");
            AddSubview(Crosshair);

            var layer = new CartoOnlineVectorTileLayer(CartoBaseMapStyle.CartoBasemapStyleVoyager);
			MapView.Layers.Add(layer);

            Crosshair.Hidden = true;

            BringSubviewToFront(Done);
            BringSubviewToFront(Cancel);
            BringSubviewToFront(AddLocation);
        }

        nfloat doneVisiblePosition, doneHiddenPosition = -1;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            MapView.Frame = Bounds;
            Popup.Frame = Bounds;

            nfloat padding = 15;

            nfloat w = 58;
            nfloat h = w;
            nfloat x = Frame.Width - (w + padding);
            nfloat y = Frame.Height - (h + padding);

            Done.Frame = new CGRect(x, y, w, h);
            Cancel.Frame = new CGRect(x, y, w, h);
            AddLocation.Frame = new CGRect(x, y, w, h);

            doneVisiblePosition = Done.Frame.X - (Done.Frame.Width + padding);
            doneHiddenPosition = Done.Frame.X;

            x = MapView.Frame.Width / 2 - w / 2;
            y = MapView.Frame.Height / 2 - h / 2;

            Crosshair.Frame = new CGRect(x, y, w, h);
        }

        public void SetCrosshairMode()
        {
            Crosshair.Hidden = false;
            AddLocation.Hidden = true;

            AnimateDoneTo(doneVisiblePosition);
        }

        public void CancelCrosshairMode()
        {
            Crosshair.Hidden = true;
            AddLocation.Hidden = false;

            AnimateDoneTo(doneHiddenPosition);
        }

        void AnimateDoneTo(nfloat to)
        {
            Animate(0.2, delegate
            {
                Done.Frame = new CGRect(to, Done.Frame.Y, Done.Frame.Width, Done.Frame.Height);   
            });
        }
    }
}
