using Carto.Core;
using Carto.Ui;
using Foundation;
using UIKit;

namespace data.collection.iOS
{
    [Register("AppDelegate")]
    public class AppDelegate : UIApplicationDelegate
    {
        const string CartoLicense = "XTUN3Q0ZGL1Z5azkvOG5DNVN5ZFcrdTNraG04bkZ" +
            "ybkxBaFEzUUVnODlzMDNzQTd0dEVmeUd1Nk1yRmhiSXc9PQoKYXBwVG9rZW49Yzh" +
            "jMGIzNGEtOWNkMi00NmM3LTk5NjgtNjU3ODJmOTUwNGMzCmJ1bmRsZUlkZW50aWZ" +
            "pZXI9Y29tLmNhcnRvLmRhdGEuY29sbGVjdGlvbgpvbmxpbmVMaWNlbnNlPTEKcHJ" +
            "vZHVjdHM9c2RrLXhhbWFyaW4taW9zLTQuKgp3YXRlcm1hcms9Y3VzdG9tCg==";

        public override UIWindow Window { get; set; }

		public UINavigationController Controller { get; set; }

		public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
        {
			MapView.RegisterLicense(CartoLicense);

            UIViewController initial = new MainController();
			Controller = new UINavigationController(initial);

			// Navigation bar background color
			Controller.NavigationBar.BarTintColor = Colors.CartoNavy;
            // Back button color
            Controller.NavigationBar.TintColor = UIColor.White;
            // Title color
            Controller.NavigationBar.TitleTextAttributes = new UIStringAttributes { 
                ForegroundColor = UIColor.White, Font = 
                    UIFont.FromName("HelveticaNeue", 15)
                };

			Controller.NavigationBarHidden = false;

			Window = new UIWindow(UIScreen.MainScreen.Bounds);

			Window.RootViewController = Controller;

			Window.MakeKeyAndVisible();

            BucketClient.Initialize();

			return true;
        }
    }
}

