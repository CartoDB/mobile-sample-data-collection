using System;
using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Widget;
using Android.Provider;

namespace data.collection.Droid
{
	public class BaseActivity : Activity
	{
		protected const int RequestCode = 1;
		protected const int Marshmallow = 23;
		
        public bool IsMarshmallow { get { return ((int)Build.VERSION.SdkInt) >= Marshmallow; } }

        public string DeviceId => Settings.Secure.GetString(ContentResolver, Settings.Secure.AndroidId);

		public void RequestPermissions(params string[] permissions)
		{
			ActivityCompat.RequestPermissions(this, permissions, RequestCode);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
		{
			if (requestCode == RequestCode)
			{
				if (grantResults.Length > 0 && grantResults[0] == Android.Content.PM.Permission.Granted)
				{
					OnPermissionsGranted();
				}
				else
				{
					OnPermissionsDenied();
				}
			}
		}

		public virtual void OnPermissionsGranted()
		{
			throw new NotImplementedException();
		}

		public virtual void OnPermissionsDenied()
		{
			throw new NotImplementedException();
		}

		protected void Alert(string message)
		{
			RunOnUiThread(delegate
			{
				Toast.MakeText(this, message, ToastLength.Short).Show();
			});
		}

		public void RunOnBackgroundThread(Action action)
		{
			System.Threading.Tasks.Task.Run(action);
		}

	}
}

