
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Carto.Core;
using Carto.DataSources;
using Carto.Layers;
using Carto.Projections;
using Carto.Utils;

namespace data.collection.Droid
{
    [Activity(Label = "DATA COLLECTION", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : BaseActivity, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        public MainView ContentView { get; set; }

        public LocationClient LocationClient { get; set; }

        LocationManager manager;

        ElementClickListener ElementListener { get; set; }
        LocationChoiceListener MapListener { get; set; }

        string[] Permissions
        {
            get
            {
                return new string[] {
                    Android.Manifest.Permission.AccessCoarseLocation,
                    Android.Manifest.Permission.AccessFineLocation
                };
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.SetSoftInputMode(SoftInput.AdjustPan);

            ContentView = new MainView(this);
            SetContentView(ContentView);

			LocationClient = new LocationClient(ContentView.MapView);

			if (IsMarshmallow)
			{
				RequestPermissions(Permissions);
			}
			else
			{
				OnPermissionsGranted();
			}

			MapListener = new LocationChoiceListener(ContentView.MapView);
            var bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.icon_pin_red);
            MapListener.Bitmap = BitmapUtils.CreateBitmapFromAndroidBitmap(bitmap);
            ElementListener = new ElementClickListener(MapListener.PointSource);

			MapListener.QueryPoints(DeviceId);
			string text = "QUERYING POINTS...";
			ContentView.Banner.SetLoadingText(text, false);
		}

        protected override void OnResume()
        {
            base.OnResume();

			ContentView.MapView.MapEventListener = MapListener;
            MapListener.PointLayer.VectorElementEventListener = ElementListener;

            LocationClient.AttachIgnoreListener();

			MapListener.PinAdded += OnPinAdded;
			MapListener.QueryFailed += OnQueryFailed;
			MapListener.PointsAdded += OnPointsAdded;

            ContentView.Content.Done.Clicked += OnDoneClick;

            ContentView.Popup.Closed += OnPopupClose;
        }

        protected override void OnPause()
        {
            base.OnPause();

			ContentView.MapView.MapEventListener = null;
            MapListener.PointLayer.VectorElementEventListener = null;

            LocationClient.DetachIgnoreListener();

			MapListener.PinAdded -= OnPinAdded;
			MapListener.QueryFailed -= OnQueryFailed;
			MapListener.PointsAdded -= OnPointsAdded;

            ContentView.Content.Done.Clicked -= OnDoneClick;

            ContentView.Popup.Closed -= OnPopupClose;
        }

        void OnPopupClose(object sender, EventArgs e)
        {
            MapListener.MarkerSource.Clear();
        }

        void OnPinAdded(object sender, EventArgs e)
        {
            MapPos position = MapListener.MarkerPosition;
            position = MapListener.Projection.ToLatLong(position.X, position.Y);

            LocationClient.MarkerLatitude = position.X;
            LocationClient.MarkerLongitude = position.Y;

            RunOnUiThread(delegate
            {
                ContentView.Popup.Show();
            });
        }

		void OnQueryFailed(object sender, EventArgs e)
		{
			string text = "CLICK ON THE MAP TO SPECIFY A LOCATION";

            RunOnUiThread(delegate
			{
				ContentView.Banner.SetInformationText(text, true);
			});
		}

        void OnPointsAdded(object sender, EventArgs e)
        {
            var syncedColor = LocationChoiceListener.SyncedLocations.ToNativeColor();
            var mySyncedColor = LocationChoiceListener.MySyncedLocations.ToNativeColor();
            var unsyncedColor = LocationChoiceListener.UnsyncedLocations.ToNativeColor();

            string text = "CLICK ON THE MAP TO SPECIFY A LOCATION";

            RunOnUiThread(delegate
            {
                ContentView.Banner.SetInformationText(text, true, delegate
                {
                    RunOnUiThread(delegate
                    {
                        ContentView.Legend.Update(syncedColor, mySyncedColor, unsyncedColor);
                    });

                });

            });
        }
		
        void OnDoneClick(object sender, EventArgs e)
		{
            // TODO implement actions from _deprecatedActivity
		}

		public override void OnPermissionsGranted()
        {
            manager = (LocationManager)GetSystemService(LocationService);

            foreach (string provider in manager.GetProviders(true))
            {
                manager.RequestLocationUpdates(provider, 1000, 50, this);
            }
        }

        public override void OnPermissionsDenied()
        {
            ContentView.Banner.SetInformationText("Fine. Locate yourself then!", true);
        }

        void RequestLocationPermission()
        {
            string fine = Android.Manifest.Permission.AccessFineLocation;
            string coarse = Android.Manifest.Permission.AccessCoarseLocation;
            ActivityCompat.RequestPermissions(this, new string[] { fine, coarse }, RequestCode);
        }

        public void OnLocationChanged(Location location)
        {
            LocationFound(location);
        }

        public void OnProviderDisabled(string provider)
        {
            Alert("Location provider disabled, bro!");
        }

        public void OnProviderEnabled(string provider)
        {
            Alert("Location provider enabled... scanning for location");
        }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Console.WriteLine("OnStatusChanged");
        }

        void LocationFound(Location location)
        {
            LocationClient.Latitude = location.Latitude;
            LocationClient.Longitude = location.Longitude;
            LocationClient.Accuracy = location.Accuracy;

            LocationClient.Update();
        }
    }
}
