using Android.App;
using Android.Widget;
using Android.OS;
using System;
using Android.Content;
using Android.Provider;
using Android.Graphics;
using Android.Locations;
using Android.Support.V4.App;
using System.IO;
using System.Collections.Generic;

namespace data.collection.Droid
{
    [Activity(Label = "DATA COLLECTION", MainLauncher = true, Icon = "@mipmap/icon")]
    public class MainActivity : BaseActivity, ILocationListener, ActivityCompat.IOnRequestPermissionsResultCallback
    {
        public MainView ContentView { get; set; }

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

			List<Data> items = SQLClient.Instance.GetAll();
			if (items.Count > 0)
			{
				ShowSyncAlert(items.Count);
			}
		}

        protected override void OnResume()
        {
            base.OnResume();

            ContentView.ImageField.Click += TakePicture;
        }

        protected override void OnPause()
        {
            base.OnPause();

            ContentView.ImageField.Click -= TakePicture;
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

        async void OnSubmitClicked()
        {
            using (var stream = new MemoryStream())
            {
                Bitmap bitmap = ContentView.ImageField.Photo;
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);

				string filename = ContentView.ImageField.ImageName;

                Data test = GetData(filename);
                SQLClient.Instance.Insert(test);

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
            string id = Settings.Secure.GetString(ContentResolver, Settings.Secure.AndroidId);
			
            var item = new Data();
            item.Identifier = id;

			item.ImageUrl = imageUrl;
			item.Title = ContentView.TitleField.Text;
			item.Description = ContentView.DescriptionField.Text;

			item.Latitude = LocationClient.Latitude;
			item.Longitude = LocationClient.Longitude;
			item.Accuracy = LocationClient.Accuracy;

			return item;
		}

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            if (requestCode == Code_TakePicture && resultCode == Result.Ok)
            {
                string name = GenerateName();
                Bitmap image = (Bitmap)data.Extras.Get("data");

                ContentView.ImageField.Photo = image;
                ContentView.ImageField.ImageName = name;

                string folder = FileUtils.GetFolder(name);

                var input = new MemoryStream();
                image.Compress(Bitmap.CompressFormat.Png, 0, input);

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
			ContentView.Banner.SetText(text, true);
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

