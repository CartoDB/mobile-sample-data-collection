
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;

namespace data.collection.Droid
{
    [Activity(Label = "CHOOSE LOCATION")]
    public class LocationChoiceActivity : BaseActivity, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        public LocationChoiceView ContentView { get; set; }

        public LocationClient LocationClient { get; set; }

        LocationManager manager;
        
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

            ContentView = new LocationChoiceView(this);
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
            // TODO
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
