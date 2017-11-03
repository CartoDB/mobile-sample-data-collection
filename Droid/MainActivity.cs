
using System;
using System.Collections.Generic;
using System.IO;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Locations;
using Android.OS;
using Android.Provider;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Carto.Core;
using Carto.Utils;

namespace data.collection.Droid
{
    [Activity(Label = "DATA COLLECTION", MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : BaseActivity, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        MainView ContentView { get; set; }

        LocationClient LocationClient { get; set; }

        LocationManager manager;

        MapClickListener MapListener { get; set; }

        PointClient PointClient { get; set; }

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

            PointClient = new PointClient(ContentView.MapView);

            var bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.icon_pin_red);
            PointClient.Bitmap = BitmapUtils.CreateBitmapFromAndroidBitmap(bitmap);

            bitmap = BitmapFactory.DecodeResource(Resources, Resource.Drawable.icon_banner_info);
            // Scale down, as our original image is too large
            int size = (int)(20 * ContentView.Density);
            bitmap = Bitmap.CreateScaledBitmap(bitmap, size, size, false);
            PointClient.PointListener.LeftImage = BitmapUtils.CreateBitmapFromAndroidBitmap(bitmap);

            PointClient.QueryPoints(delegate { });

            MapListener = new MapClickListener();

            List<Data> items = SQLClient.Instance.GetAll();
            if (items.Count > 0)
            {
                ShowSyncAlert(items.Count);
            }

            //var url = "https://s3.amazonaws.com/com.carto.mobile.images/dde1cfd1-1538-499c-b9ed-6a52ff0e9b9d.png";
            //Networking.GetImage(url);
        }

        protected override void OnResume()
        {
            base.OnResume();

            LocationClient.AttachIgnoreListener();

            PointClient.QueryFailed += OnQueryFailed;
            PointClient.PointsAdded += OnPointsAdded;

            ContentView.Content.Done.Clicked += OnDoneClick;
            ContentView.Popup.Closed += OnPopupClose;

            ContentView.Content.CameraField.Click += TakePicture;

            ContentView.Add.Clicked += OnAddLocationClick;
            ContentView.Done.Clicked += OnLocationChosen;
            ContentView.Cancel.Clicked += OnLocationChoiceCancelled;

            ContentView.Popup.Closed += OnPopupClosed;

            ContentView.MapView.MapEventListener = MapListener;
            MapListener.MapClicked += OnMapClicked;

            PointClient.PointListener.Click += OnPointClicked;
        }

        protected override void OnPause()
        {
            base.OnPause();

            LocationClient.DetachIgnoreListener();

            PointClient.QueryFailed -= OnQueryFailed;
            PointClient.PointsAdded -= OnPointsAdded;

            ContentView.Content.Done.Clicked -= OnDoneClick;
            ContentView.Popup.Closed -= OnPopupClose;

            ContentView.Content.CameraField.Click -= TakePicture;

            ContentView.Add.Clicked -= OnAddLocationClick;
            ContentView.Done.Clicked -= OnLocationChosen;
            ContentView.Cancel.Clicked -= OnLocationChoiceCancelled;

            ContentView.Popup.Closed += OnPopupClosed;

            ContentView.MapView.MapEventListener = null;
            MapListener.MapClicked -= OnMapClicked;

            PointClient.PointListener.Click -= OnPointClicked;
        }

        async void OnPointClicked(object sender, EventArgs e)
        {
            var url = (string)sender;
            ImageResponse response = await Networking.GetImage(url);

            if (response.IsOk)
            {
                Bitmap bitmap = BitmapFactory.DecodeStream(response.Stream);

                RunOnUiThread(delegate {
                    ContentView.Attachment.SetImage(bitmap);
                    ContentView.Attachment.Show();    
                });

            }
            else
            {
                var text = "Unable to load element image";
                RunOnUiThread(delegate {
                    ContentView.Banner.SetInformationText(text, true);    
                });
            }
        }

        void OnMapClicked(object sender, EventArgs e)
        {
            PointClient.PopupSource.Clear();

            RunOnUiThread(delegate {
                ContentView.Attachment.Hide();    
            });
        }

        void OnPopupClosed(object sender, EventArgs e)
        {
            ContentView.CancelCrosshairMode();
        }

        void OnAddLocationClick(object sender, EventArgs e)
        {
            ContentView.SetCrosshairMode();
        }

        void OnLocationChosen(object sender, EventArgs e)
        {
            // Crosshair is a regular ImageView centered on the MapView,
            // Translate crosshair's coordinates to a position on the map
            var parameters = (RelativeLayout.LayoutParams)ContentView.Crosshair.LayoutParameters;
            var x = parameters.LeftMargin + parameters.Width / 2;
            var y = parameters.TopMargin + parameters.Height / 2;
            var screen = new ScreenPos(x, y);
            ContentView.MapView.ScreenToMap(screen);

            MapPos position = ContentView.MapView.ScreenToMap(screen);
            PointClient.AddUserMarker(position);

            // Center marker on currently visible area (partically hidden by popup)
            var mapBounds = new MapBounds(position, position);
            y = ContentView.Popup.VisibleY / 2;
            screen = new ScreenPos(x, y);
            var screenBounds = new ScreenBounds(screen, screen);
            ContentView.MapView.MoveToFitBounds(mapBounds, screenBounds, false, 0.2f);

            // Translate internal units to lat/lon
            position = PointClient.Projection.ToLatLong(position.X, position.Y);

            LocationClient.MarkerLatitude = position.X;
            LocationClient.MarkerLongitude = position.Y;

            RunOnUiThread(delegate
            {
                ContentView.Popup.Show();
            });
        }

        void OnLocationChoiceCancelled(object sender, EventArgs e)
        {
            ContentView.CancelCrosshairMode();
        }

        void OnPopupClose(object sender, EventArgs e)
        {
            PointClient.MarkerSource.Clear();
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
            var syncedColor = PointClient.SyncedLocations.ToNativeColor();
            var mySyncedColor = PointClient.MySyncedLocations.ToNativeColor();
            var unsyncedColor = PointClient.UnsyncedLocations.ToNativeColor();

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

            ContentView.CancelCrosshairMode();

            PointClient.MarkerSource.Clear();
            ContentView.Popup.Hide();
            ContentView.Banner.SetInformationText("Compressing image...", false);

            using (var stream = new MemoryStream())
            {
                Bitmap bitmap = ContentView.Content.CameraField.Photo;

                BucketResponse response1;
                string filename;

                if (bitmap == null)
                {
                    // Photo is an optional field. 
                    // Create a mock successful response, 
                    // if the user hasn't taken a photo
                    response1 = new BucketResponse();
                    response1.Path = "";
                    filename = "";
                }
                else
                {
                    bitmap.Compress(Bitmap.CompressFormat.Png, Quality, stream);

                    filename = ContentView.Content.CameraField.ImageName;

                    ContentView.Banner.ShowUploadingImage();

                    response1 = await BucketClient.Upload(filename, stream);
                }

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

                PointClient.QueryPoints(delegate { });
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

        #region SyncAlert

        public void ShowSyncAlert(int count)
        {
            string title = "Attention!";
            string message = "You have " + count + " items stored locally. Would you like to upload now?";

            var builder = new AlertDialog.Builder(this);
            builder.SetTitle(title);
            builder.SetMessage(message);
            builder.SetCancelable(false);

            builder.SetPositiveButton("SURE", OnYesButtonClicked);
            builder.SetNegativeButton("NO", OnNoButtonClicked);
            builder.Show();
        }

        async void OnYesButtonClicked(object sender, DialogClickEventArgs e)
        {
            List<Data> items = SQLClient.Instance.GetAll();
            int count = items.Count;

            ContentView.Banner.ShowUploadingEverything(count);

            foreach (Data item in items)
            {
                if (!item.IsUploadedToAmazon)
                {
                    if (item.ImageUrl != "")
                    {
                        byte[] bytes = File.ReadAllBytes(FileUtils.GetFolder(item.ImageUrl));
                        Stream stream = new MemoryStream(bytes);
                        BucketResponse response1 = await BucketClient.Upload(item.FileName, stream);

                        if (response1.IsOk)
                        {
                            item.ImageUrl = response1.Path;
                            Console.WriteLine("Uploaded offline image to: " + response1.Path);
                        }    
                    }
                }
            }

            CartoResponse response2 = await Networking.Post(items);

            if (response2.IsOk)
            {
                ContentView.Banner.ShowUploadedEverything(count);
                SQLClient.Instance.DeleteAll();
            }
            else
            {
                ContentView.Banner.ShowEverythingUploadFailed(count);
                SQLClient.Instance.UpdateAll(items);
            }
        }

        void OnNoButtonClicked(object sender, DialogClickEventArgs e)
        {
            (sender as AlertDialog).Cancel();

            string text = "Fine. We'll just keep your stuff offline then";
            ContentView.Banner.SetInformationText(text, true);
        }

        #endregion
    }
}
