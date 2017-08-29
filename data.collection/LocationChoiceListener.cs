
using System;
using Carto.Core;
using Carto.DataSources;
using Carto.Graphics;
using Carto.Layers;
using Carto.Projections;
using Carto.Styles;
using Carto.Ui;
using Carto.VectorElements;

namespace data.collection
{
    public class LocationChoiceListener : MapEventListener
    {
        public EventHandler<EventArgs> PinAdded;

        public MapView MapView { get; private set; }

        public Bitmap Bitmap { get; set; }

        LocalVectorDataSource source;

        public Projection Projection
        {
            get { return MapView.Options.BaseProjection; }
        }

        public MapPos MarkerPosition { get; set; }

        public LocationChoiceListener(MapView mapView)
        {
            MapView = mapView;

			source = new LocalVectorDataSource(Projection);
            VectorLayer routeLayer = new VectorLayer(source);
			MapView.Layers.Add(routeLayer);
		}

        public override void OnMapClicked(MapClickInfo mapClickInfo)
        {
            source.Clear();

            MarkerPosition = mapClickInfo.ClickPos;
            var marker = GetUserMarker(Bitmap, MarkerPosition);
            source.Add(marker);

            PinAdded?.Invoke(this, EventArgs.Empty);
        }

        public static Marker GetUserMarker(Bitmap bitmap, MapPos position)
        {
            var builder = new MarkerStyleBuilder();
            builder.Bitmap = bitmap;
            builder.HideIfOverlapped = false;
            builder.Size = 30;
            builder.Color = new Color(0, 255, 0, 255);

            var animationBuilder = new AnimationStyleBuilder();
            animationBuilder.SizeAnimationType = AnimationType.AnimationTypeSpring;
            builder.AnimationStyle = animationBuilder.BuildStyle();

            return new Marker(position, builder.BuildStyle());
        }
    }
}
