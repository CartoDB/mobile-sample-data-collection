
using System;
using Android.App;
using Android.Runtime;
using Carto.Ui;
using Carto.Utils;

namespace data.collection.Droid
{
	[Application]
	public class CollectionApplication : Application
	{
		const string CartoLicense = "XTUN3Q0ZHZ2IwVnRhR21maXdKaGphcDZFd0tIN" +
            "1FIQ29BaFFPUVFXTzlkOHVVOHQvRll0eGpMZ2cvek00a2c9PQoKYXBwVG9rZW4" +
            "9ZjI2MTc0NDMtNWI4Zi00ZDU4LWE3ZjEtMTM1YmQ4NzFhODJmCnBhY2thZ2VOY" +
            "W1lPWNvbS5jYXJ0by5kYXRhLmNvbGxlY3Rpb24Kb25saW5lTGljZW5zZT0xCn" +
            "Byb2R1Y3RzPXNkay14YW1hcmluLWFuZHJvaWQtNC4qCndhdGVybWFyaz1jdXN0b20K";
        
		public CollectionApplication(IntPtr a, JniHandleOwnership b) : base(a, b)
		{
            
		}

		public override void OnCreate()
		{
			base.OnCreate();

			Log.ShowInfo = true;
			Log.ShowError = true;
			Log.ShowWarn = true;

			MapView.RegisterLicense(CartoLicense, ApplicationContext);

            BucketClient.Initialize(Assets);
		}
	}

}

