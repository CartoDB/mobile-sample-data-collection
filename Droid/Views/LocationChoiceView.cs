
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
using Carto.Layers;
using Carto.Ui;

namespace data.collection.Droid
{
    public class LocationChoiceView : BaseView
    {
		public MapView MapView { get; private set; }

		public LocationChoiceView(Context context) : base(context)
        {
			MapView = new MapView(context);
			AddView(MapView);

            SetMainViewFrame();

			var layer = new CartoOnlineVectorTileLayer(CartoBaseMapStyle.CartoBasemapStyleVoyager);
			MapView.Layers.Add(layer);
		}

		public override void LayoutSubviews()
		{
			MapView.SetFrame(0, 0, Frame.W, Frame.H);
		}

	}
}
