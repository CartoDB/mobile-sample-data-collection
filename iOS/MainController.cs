
using System;
using System.Collections.Generic;
using System.IO;
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

            List<Data> items = SQLClient.Instance.GetAll();
            if (items.Count > 0)
            {
                ShowSyncAlert(items.Count);
            }

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

			PointClient.QueryFailed += OnQueryFailed;
			PointClient.PointsAdded += OnPointsAdded;

            ContentView.AddLocation.Click += OnAddLocationClick;
            ContentView.Done.Click += OnLocationChosen;
            ContentView.Cancel.Click += OnLocationChoiceCancelled;

            PointClient.PointListener.Click += OnPointClicked;

            ContentView.MapView.MapEventListener = MapListener;
            MapListener.MapClicked += OnMapClicked;

            ContentView.Popup.Closed += OnPopupClosed;

            ContentView.Content.CameraField.Click += OnCameraButtonClick;
            Camera.Instance.Delegate.Complete += OnCameraActionComplete;

            ContentView.Content.Done.Click += OnDoneClick;
		}

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            LocationClient.DetachIgnoreListener();

            LocationManager.Stop();
            LocationManager.LocationUpdated -= OnLocationUpdate;

            PointClient.QueryFailed -= OnQueryFailed;
            PointClient.PointsAdded -= OnPointsAdded;

            ContentView.AddLocation.Click -= OnAddLocationClick;
            ContentView.Done.Click -= OnLocationChosen;
            ContentView.Cancel.Click -= OnLocationChoiceCancelled;

            PointClient.PointListener.Click -= OnPointClicked;

            ContentView.MapView.MapEventListener = null;
            MapListener.MapClicked -= OnMapClicked;

            ContentView.Popup.Closed -= OnPopupClosed;

            ContentView.Content.CameraField.Click -= OnCameraButtonClick;
            Camera.Instance.Delegate.Complete -= OnCameraActionComplete;

            ContentView.Content.Done.Click -= OnDoneClick;
        }

        void OnCameraButtonClick(object sender, EventArgs e)
        {
            Camera.Instance.TakePicture(this);
        }

        void OnCameraActionComplete(object sender, PhotoEventArgs e)
        {
            UIImage image = Camera.GetImageFromInfo(e.Info);

            image = ImageUtils.Resize(image);

            ContentView.Content.CameraField.SetPhoto(image);

            string filename = GenerateName();
            ContentView.Content.CameraField.ImageName = filename;

            InvokeInBackground(delegate
            {
                Camera.SaveImage(image, filename);
            });
        }

        string GenerateName()
        {
            return Guid.NewGuid().ToString() + Camera.Extension;
        }

        void OnPopupClosed(object sender, EventArgs e)
        {
            PointClient.MarkerSource.Clear();
            ContentView.CancelCrosshairMode();

            ContentView.Content.EndEditing(true);

            ContentView.Content.Clear();
        }

        void OnMapClicked(object sender, EventArgs e)
        {
            PointClient.PopupSource.Clear();

            InvokeOnMainThread(delegate
            {
                ContentView.Attachment.Hide();
                ContentView.Attachment.Collapse(false);
            });
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
                InvokeOnMainThread(delegate
                {
                    ContentView.Attachment.ShowPlaceholder();
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
            var y = (parameters.Y + parameters.Height / 2 - Device.TrueY0) * scale;
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
                ContentView.CancelCrosshairMode();
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

        async void OnDoneClick(object sender, EventArgs e)
        {
            if (ContentView.IsAnyRequiredFieldEmpty)
            {
                string text = "Please fill out all required fields";
                ContentView.Banner.SetInformationText(text, true);
                return;
            }

            ContentView.CancelCrosshairMode();
            PointClient.MarkerSource.Clear();
            ContentView.Popup.Hide();

            UIImage image = ContentView.Content.CameraField.Photo.Image;

            BucketResponse response1;

            if (image == null)
            {
                // Photo is an optional field. 
                // Create a mock successful response, 
                // if the user hasn't taken a photo
                response1 = new BucketResponse();
                response1.Path = "";
            }
            else
            {

                Stream stream = Camera.GetStreamFromImage(image);
                ContentView.Banner.ShowUploadingImage();
                string filename = ContentView.Content.CameraField.ImageName;
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
                Data item = GetData(Camera.LatestImageName);
                SQLClient.Instance.Insert(item);
            }

            PointClient.QueryPoints(delegate { });
        }

        public Data GetData(string url)
        {
            string id = Device.Id;

            string title = ContentView.Content.TitleField.Text;
            string description = ContentView.Content.DescriptionField.Text;

            return Data.Get(id, url, title, description);
        }

        public void ShowSyncAlert(int count)
        {
            string title = "Attention!";
            string description = "You have " + count + " items stored locally. Would you like to upload now?";
            var controller = UIAlertController.Create(title, description, UIAlertControllerStyle.Alert);

            controller.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, OnUploadAlertOK));
            controller.AddAction(UIAlertAction.Create("Cancel", UIAlertActionStyle.Cancel, OnUploadAlertCancel));

            PresentViewController(controller, true, null);
        }

        public async void OnUploadAlertOK(UIAlertAction action)
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

        public void OnUploadAlertCancel(UIAlertAction action)
        {
            string text = "Fine. We'll just keep your stuff offline then";
            ContentView.Banner.SetInformationText(text, true);
        }
    }
}
