
using System;
using Carto.Core;
using Carto.Utils;
using UIKit;

namespace data.collection.iOS
{
    public class LocationChoiceController : UIViewController
    {
        public LocationChoiceView ContentView { get; set; }

		LocationManager LocationManager { get; set; }

		LocationClient LocationClient { get; set; }

        LocationChoiceListener Listener { get; set; }

		public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ContentView = new LocationChoiceView();
            View = ContentView;

			LocationManager = new LocationManager();

			LocationClient = new LocationClient(ContentView.MapView);

            Listener = new LocationChoiceListener(ContentView.MapView);
            Listener.Bitmap = BitmapUtils.CreateBitmapFromUIImage(UIImage.FromFile("icon_pin_red.png"));

			string text = "CLICK ON THE MAP TO SPECIFY A LOCATION";
			ContentView.Banner.SetInformationText(text, false);

            Listener.QueryPoints(Device.Id);
		}

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            LocationClient.AttachIgnoreListener();

			LocationManager.Start();
			LocationManager.LocationUpdated += OnLocationUpdate;

            ContentView.MapView.MapEventListener = Listener;

            Listener.PinAdded += OnPinAdded;

            ContentView.Done.Click += OnDoneClick;
		}

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            LocationClient.DetachIgnoreListener();

            LocationManager.Stop();
            LocationManager.LocationUpdated -= OnLocationUpdate;

            ContentView.MapView.MapEventListener = null;

            Listener.PinAdded -= OnPinAdded;

            ContentView.Done.Click -= OnDoneClick;
        }

        void OnPinAdded(object sender, EventArgs e)
        {
            MapPos position = Listener.MarkerPosition;
            position = Listener.Projection.ToLatLong(position.X, position.Y);

            LocationClient.MarkerLatitude = position.X;
            LocationClient.MarkerLongitude = position.Y;

            InvokeOnMainThread(delegate
            {
                ContentView.Done.Hidden = false;
            });
        }

        void OnLocationUpdate(object sender, LocationUpdatedEventArgs e)
        {
            LocationClient.Latitude = e.Location.Coordinate.Latitude;
            LocationClient.Longitude = e.Location.Coordinate.Longitude;
            LocationClient.Accuracy = e.Location.HorizontalAccuracy;

            LocationClient.Update();
        }

        void OnDoneClick(object sender, EventArgs e)
        {
            NavigationController.PopViewController(true);
        }
    }
}
