using Android.App;
using Android.OS;
using System;
using Android.Content;
using Android.Provider;
using Android.Graphics;
using System.IO;
using System.Collections.Generic;

namespace data.collection.Droid
{
    [Activity(Label = "DATA COLLECTION", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : BaseActivity
    {
        public MainView ContentView { get; set; }

		protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ContentView = new MainView(this);
            SetContentView(ContentView);

			List<Data> items = SQLClient.Instance.GetAll();
			if (items.Count > 0)
			{
				ShowSyncAlert(items.Count);
			}
		}

        protected override void OnResume()
        {
            base.OnResume();

            ContentView.PhotoField.Click += TakePicture;
            Settings.Secure.GetString(ContentResolver, Settings.Secure.AndroidId);
            ContentView.LocationField.Click += AddLocation;

			if (LocationClient.IsMarkerSet)
			{
				ContentView.AddMapOverlayTo(
					LocationClient.MarkerLongitude, LocationClient.MarkerLatitude
				);
			}
        }

        protected override void OnPause()
        {
            base.OnPause();

            ContentView.PhotoField.Click -= TakePicture;

            ContentView.LocationField.Click -= AddLocation;
        }

        public override bool OnCreateOptionsMenu(Android.Views.IMenu menu)
        {
            menu.Add(new Java.Lang.String(""))
                .SetIcon(Resource.Drawable.icon_done)
                .SetShowAsAction(Android.Views.ShowAsAction.Always);

            return true;
        }

        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            if (item.ItemId == 0)
            {
                OnSubmitClicked();
            }

            return true;
        }

        const int Code_TakePicture = 1;

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

        void AddLocation(object sender, EventArgs e)
        {
            StartActivity(typeof(LocationChoiceActivity));  
        }

		// Quality Accepts 0 - 100:
		// 0 = MAX Compression(Least Quality which is suitable for Small images)
		// 100 = Least Compression(MAX Quality which is suitable for Big images)
        const int Quality = 100;

		async void OnSubmitClicked()
        {
            ContentView.Banner.SetInformationText("Compressing image...", false);

            using (var stream = new MemoryStream())
            {
                Bitmap bitmap = ContentView.PhotoField.Photo;
                bitmap.Compress(Bitmap.CompressFormat.Png, Quality, stream);

				string filename = ContentView.PhotoField.ImageName;

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
            }
        }

		public Data GetData(string imageUrl)
		{
            string id = DeviceId;
            string title = ContentView.TitleField.Text;
            string description = ContentView.DescriptionField.Text;

            return Data.Get(id, imageUrl, title, description);
		}

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == Code_TakePicture && resultCode == Result.Ok)
            {
                string name = GenerateName();
                Bitmap image = (Bitmap)data.Extras.Get("data");

                ContentView.PhotoField.Photo = image;
                ContentView.PhotoField.ImageName = name;

                string folder = FileUtils.GetFolder(name);

                var input = new MemoryStream();
                image.Compress(Bitmap.CompressFormat.Png, Quality, input);

                var output = File.Create(folder);

                input.Seek(0, SeekOrigin.Begin);
                input.CopyTo(output);
            }
        }

        const string Extension = ".png";
		string GenerateName()
		{
			return Guid.NewGuid().ToString() + Extension;
		}

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

		void OnNoButtonClicked(object sender, DialogClickEventArgs e)
		{
            (sender as AlertDialog).Cancel();
			
            string text = "Fine. We'll just keep your stuff offline then";
			ContentView.Banner.SetInformationText(text, true);
		}
    }
}

