
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Provider;
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
    [Activity(Label = "DATA COLLECTION", MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = ScreenOrientation.Portrait)]
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

            ContentView.Content.CameraField.Click += TakePicture;
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

            ContentView.Content.CameraField.Click -= TakePicture;
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
                ContentView.Banner.SetInformationText(text, true);

            });
        }

        async void OnDoneClick(object sender, EventArgs e)
        {
            if (ContentView.IsAnyRequiredFieldEmpty)
            {
                ContentView.Banner.SetInformationText("Please fill our all required fields", true);
                return;
            }

            MapListener.MarkerSource.Clear();
            ContentView.Popup.Hide();
            ContentView.Banner.SetInformationText("Compressing image...", false);

            using (var stream = new MemoryStream())
            {
                Bitmap bitmap = ContentView.Content.CameraField.Photo;
                bitmap.Compress(Bitmap.CompressFormat.Png, Quality, stream);

                string filename = ContentView.Content.CameraField.ImageName;

                ContentView.Banner.ShowUploadingImage();

                BucketResponse response1 = await BucketClient.Upload(filename, stream);

                if (response1.IsOk)
                {
                    ContentView.Banner.ShowUploadingData();

                    Data item = GetData(response1.Path);
                    CartoResponse response2 = await Networking.Post(item);

                    if (response2.IsOk)
                    {
                        ContentView.Banner.Complete();
                    }
                    else
                    {
                        ContentView.Banner.ShowFailedCartoUpload();
                        SQLClient.Instance.Insert(item);
                    }
                }
                else
                {
                    ContentView.Banner.ShowFailedAmazonUpload();
                    Data item = GetData(filename);
                    SQLClient.Instance.Insert(item);
                }

                MapListener.QueryPoints(DeviceId);
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

        public Data GetData(string imageUrl)
        {
            string id = DeviceId;
            string title = ContentView.Content.TitleField.Text;
            string description = ContentView.Content.DescriptionField.Text;

            return Data.Get(id, imageUrl, title, description);
        }

        #region Camera

        const int Code_TakePicture = 1;

        // Quality Accepts 0 - 100:
        // 0 = MAX Compression(Least Quality which is suitable for Small images)
        // 100 = Least Compression(MAX Quality which is suitable for Big images)
        const int Quality = 100;

        const string Extension = ".png";

        void TakePicture(object sender, EventArgs e)
        {
            var intent = new Intent(MediaStore.ActionImageCapture);

            // Most basic camera implementation from:
            // https://developer.android.com/training/camera/photobasics.html
            if (intent.ResolveActivity(PackageManager) != null)
            {
                StartActivityForResult(intent, Code_TakePicture);
            }
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == Code_TakePicture && resultCode == Result.Ok)
            {
                string name = GenerateName();
                Bitmap image = (Bitmap)data.Extras.Get("data");

                ContentView.Content.CameraField.Photo = image;
                ContentView.Content.CameraField.ImageName = name;

                string folder = FileUtils.GetFolder(name);

                var input = new MemoryStream();
                image.Compress(Bitmap.CompressFormat.Png, Quality, input);

                var output = File.Create(folder);

                input.Seek(0, SeekOrigin.Begin);
                input.CopyTo(output);
            }
        }

        string GenerateName()
        {
            return Guid.NewGuid().ToString() + Extension;
        }

        #endregion

    }
}
