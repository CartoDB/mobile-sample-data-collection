
using System;
using System.IO;
using System.Collections.Generic;
using UIKit;

namespace data.collection.iOS
{
    public class MainController : UIViewController
    {
        MainView ContentView { get; set; }

		public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            ContentView = new MainView();
            View = ContentView;

            Title = "DATA COLLECTION";

            List<Data> items = SQLClient.Instance.GetAll();
            if (items.Count > 0)
            {
                ShowSyncAlert(items.Count);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            Camera.Instance.Delegate.Complete += OnCameraActionComplete;

            ContentView.CameraField.AddGestureRecognizer(OnCameraButtonClick);
            ContentView.LocationField.AddGestureRecognizer(OnLocationButtonClick);

            ContentView.Submit.Click += OnSubmitClicked;

            if (LocationClient.IsMarkerSet)
            {
                ContentView.AddMapOverlayTo(
                    LocationClient.MarkerLongitude, LocationClient.MarkerLatitude
                );
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            Camera.Instance.Delegate.Complete -= OnCameraActionComplete;

            ContentView.CameraField.RemoveGestureRecognizer();
            ContentView.LocationField.RemoveGestureRecognizer();

            ContentView.Submit.Click -= OnSubmitClicked;
        }

        public Data GetData(string imageUrl)
        {
            var item = new Data();
            item.Identifier = UIDevice.CurrentDevice.IdentifierForVendor.ToString();

            item.ImageUrl = imageUrl;
            item.Title = ContentView.TitleField.Text;
            item.Description = ContentView.DescriptionField.Text;
			
            item.Latitude = LocationClient.Latitude;
			item.Longitude = LocationClient.Longitude;
			item.Accuracy = LocationClient.Accuracy;

            return item;
        }

        string GenerateName()
        {
            return Guid.NewGuid().ToString() + Camera.Extension;
        }

        async void OnSubmitClicked(object sender, EventArgs e)
        {
            if (ContentView.IsAnyFieldEmpty)
            {
                ContentView.Banner.SetInformationText("Please fill our all fields", true);
                return;
            }

            UIImage image = ContentView.CameraField.Photo.Image;

            if (image == null)
            {
                ContentView.Banner.SetInformationText("Please set an image before submitting", true);
                return;
            }

            Stream stream = Camera.GetStreamFromImage(image);

            ContentView.Banner.ShowUploadingImage();

            string filename = ContentView.CameraField.ImageName;

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
				Data item = GetData(Camera.LatestImageName);
				SQLClient.Instance.Insert(item);
            }
        }

        void OnCameraButtonClick()
		{
			Camera.Instance.TakePicture(this);
		}

        void OnLocationButtonClick()
        {
            var controller = new LocationChoiceController();
            NavigationController.PushViewController(controller, true);
        }

		void OnCameraActionComplete(object sender, PhotoEventArgs e)
        {
            UIImage image = Camera.GetImageFromInfo(e.Info);
            ContentView.CameraField.Photo.Image = image;

            string filename = GenerateName();
            ContentView.CameraField.ImageName = filename;

			InvokeInBackground(delegate
			{
				Camera.SaveImage(image, filename);
			});
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
