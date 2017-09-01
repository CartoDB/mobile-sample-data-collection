
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace data.collection.Droid
{
    public class BannerView : BaseView
    {
		public Banner Banner { get; private set; }

		public BannerView(Context context) : base(context)
        {
			Banner = new Banner(context);
			AddView(Banner);

		}

        protected int padding;

		public override void LayoutSubviews()
        {
			int x = 0;
			int y = 0;
			int h = (int)(45 * Density);
			int w = Frame.W;

			padding = (int)(15 * Density);

			Banner.Frame = new CGRect(x, y, w, h);
		}
    }
}
