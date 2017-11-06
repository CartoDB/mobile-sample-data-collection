
using System;
using Carto.Core;
using Carto.Utils;
using UIKit;

namespace data.collection.iOS
{
    public class MainController : UIViewController
    {
        public MainView ContentView { get; set; }

		LocationManager LocationManager { get; set; }

		LocationClient LocationClient { get; set; }

        PointClient PointClient { get; set; }

        MapClickListener MapListener { get; set; }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ContentView = new MainView();
            View = ContentView;

            LocationManager = new LocationManager();

            LocationClient = new LocationClient(ContentView.MapView);

            PointClient = new PointClient(ContentView.MapView);
            PointClient.Bitmap = BitmapUtils.CreateBitmapFromUIImage(UIImage.FromFile("icon_pin_red.png"));

            MapListener = new MapClickListener();

            string text = "QUERYING POINTS...";
            ContentView.Banner.SetLoadingText(text, false);

            PointClient.QueryPoints(delegate
            {
                InvokeOnMainThread(delegate
                {
                    ContentView.Banner.Hide();
                });

            });
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            LocationClient.AttachIgnoreListener();

			LocationManager.Start();
			LocationManager.LocationUpdated += OnLocationUpdate;

            ContentView.MapView.MapEventListener = MapListener;

			PointClient.QueryFailed += OnQueryFailed;
			PointClient.PointsAdded += OnPointsAdded;

			ContentView.Done.Click += OnDoneClick;
		}

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            LocationClient.DetachIgnoreListener();

            LocationManager.Stop();
            LocationManager.LocationUpdated -= OnLocationUpdate;

            ContentView.MapView.MapEventListener = null;

            PointClient.QueryFailed -= OnQueryFailed;
            PointClient.PointsAdded -= OnPointsAdded;

            ContentView.Done.Click -= OnDoneClick;
        }

        void OnPinAdded(object sender, EventArgs e)
        {
            MapPos position = PointClient.MarkerPosition;
            position = PointClient.Projection.ToLatLong(position.X, position.Y);

            LocationClient.MarkerLatitude = position.X;
            LocationClient.MarkerLongitude = position.Y;

            InvokeOnMainThread(delegate
            {
                ContentView.Done.Hidden = false;
            });
        }

        void OnQueryFailed(object sender, EventArgs e)
        {
			string text = "CLICK ON THE MAP TO SPECIFY A LOCATION";

            InvokeOnMainThread(delegate
            {
                ContentView.Banner.SetInformationText(text, false);
            });
        }

        void OnPointsAdded(object sender, EventArgs e)
        {
            var syncedColor = PointClient.SyncedLocations.ToNativeColor();
            var mySyncedColor = PointClient.MySyncedLocations.ToNativeColor();
            var unsyncedColor = PointClient.UnsyncedLocations.ToNativeColor();
			
            string text = "CLICK ON THE MAP TO SPECIFY A LOCATION";

            InvokeOnMainThread(delegate
            {
                ContentView.Banner.SetInformationText(text, false);
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
