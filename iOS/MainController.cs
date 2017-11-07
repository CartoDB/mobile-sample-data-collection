
using System;
using Carto.Core;
using Carto.Utils;
using Foundation;
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

            Title = "Data Collection";

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

            ContentView.AddLocation.Click += OnAddLocationClick;
            ContentView.Done.Click += OnLocationChosen;
            ContentView.Cancel.Click += OnLocationChoiceCancelled;

            PointClient.PointListener.Click += OnPointClicked;
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

            ContentView.AddLocation.Click -= OnAddLocationClick;
            ContentView.Done.Click -= OnLocationChosen;
            ContentView.Cancel.Click -= OnLocationChoiceCancelled;

            PointClient.PointListener.Click -= OnPointClicked;
        }

        async void OnPointClicked(object sender, EventArgs e)
        {
            InvokeOnMainThread(delegate
            {
                ContentView.Attachment.Show();
            });

            var url = (string)sender;
            ImageResponse response = await Networking.GetImage(url);

            if (response.IsOk)
            {
                UIImage bitmap = new UIImage(NSData.FromStream(response.Stream));

                InvokeOnMainThread(delegate
                {
                    ContentView.Attachment.SetImage(bitmap);
                });
            }
            else
            {
                var text = "Unable to load element image";
                InvokeOnMainThread(delegate
                {
                    ContentView.Banner.SetInformationText(text, true);
                    ContentView.Attachment.Hide();
                });
            }
        }

        void OnAddLocationClick(object sender, EventArgs e)
        {
            ContentView.SetCrosshairMode();
        }

        void OnLocationChosen(object sender, EventArgs e)
        {
            // Scale needs to be accounted for separetly,
            // multiply all iOS view coordinates by scale to get real values
            nfloat scale = UIScreen.MainScreen.Scale;

            // Crosshair is a regular ImageView centered on the MapView,
            // Translate crosshair's coordinates to a position on the map
            var parameters = ContentView.Crosshair.Frame;
            var x = (parameters.X + parameters.Width / 2) * scale;
            var y = (parameters.Y + parameters.Height / 2) * scale;
            var screen = new ScreenPos((float)x, (float)y);
            ContentView.MapView.ScreenToMap(screen);

            MapPos position = ContentView.MapView.ScreenToMap(screen);
            PointClient.AddUserMarker(position);

            // Center marker on currently visible area (partically hidden by popup)
            var mapBounds = new MapBounds(position, position);
            y = (ContentView.Popup.VisibleY / 2) * scale;
            screen = new ScreenPos((float)x, (float)y);
            var screenBounds = new ScreenBounds(screen, screen);
            ContentView.MapView.MoveToFitBounds(mapBounds, screenBounds, false, 0.2f);

            // Translate internal units to lat/lon
            position = PointClient.Projection.ToLatLong(position.X, position.Y);

            LocationClient.MarkerLatitude = position.X;
            LocationClient.MarkerLongitude = position.Y;

            InvokeOnMainThread(delegate
            {
                ContentView.Popup.Show();
            });
        }

        void OnLocationChoiceCancelled(object sender, EventArgs e)
        {
            ContentView.CancelCrosshairMode();
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
