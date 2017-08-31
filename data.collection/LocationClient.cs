using System;
using Carto.Core;
using Carto.DataSources;
using Carto.Geometry;
using Carto.Graphics;
using Carto.Layers;
using Carto.Projections;
using Carto.Styles;
using Carto.Ui;
using Carto.VectorElements;

namespace data.collection
{
    public class LocationClient
    {
		public static double Latitude, Longitude, Accuracy = -1;
        public static double MarkerLatitude, MarkerLongitude = -500;

        public static bool IsMarkerSet => MarkerLongitude > -500;

        static readonly Color LightTransparentAppleBlue = new Color(14, 122, 254, 70);
        static readonly Color DarkTransparentAppleBlue = new Color(14, 122, 254, 150);

        public MapView MapView { get; private set; }

        Point userMarker;
        Polygon accuracyMarker;

        public Projection Projection
        { 
            get { return MapView.Options.BaseProjection; }
        }

        LocalVectorDataSource source;
        VectorLayer layer;

        public LocationClient(MapView mapView)
        {
            MapView = mapView;

            source = new LocalVectorDataSource(Projection);
            layer = new VectorLayer(source);
            MapView.Layers.Add(layer);

            listener = new VectorElementIgnoreListener();
        }

        VectorElementIgnoreListener listener;

        public void AttachIgnoreListener()
        {
            layer.VectorElementEventListener = listener;
        }

        public void DetachIgnoreListener()
        {
            layer.VectorElementEventListener = null;
        }

		/*
         * If foundUser is true, MapView won't zoom or focus anymore
         */
        public bool FoundUser { get; private set; }

        public void Update()
        {
            MapPos position = Projection.FromWgs84(new MapPos(Longitude, Latitude));

            if (!FoundUser)
            {
				MapView.SetFocusPos(position, 0);
				MapView.SetZoom(17, 0);
                FoundUser = true;
            }

            var builder = new PolygonStyleBuilder();
            builder.Color = LightTransparentAppleBlue;

            var borderBuilder = new LineStyleBuilder();
            borderBuilder.Color = DarkTransparentAppleBlue;
            borderBuilder.Width = 1;

            MapPosVector points = GetCirclePoints(Longitude, Latitude, Accuracy);

            if (accuracyMarker == null)
            {
                accuracyMarker = new Polygon(points, new MapPosVectorVector(), builder.BuildStyle());
                source.Add(accuracyMarker);
            } 
            else
            {
                accuracyMarker.Style = builder.BuildStyle();
                accuracyMarker.Geometry = new PolygonGeometry(points);
            }

            if (userMarker == null)
            {
                var markerBuilder = new PointStyleBuilder();
                markerBuilder.Color = DarkTransparentAppleBlue;
                markerBuilder.Size = 16.0f;

                userMarker = new Point(position, markerBuilder.BuildStyle());
                source.Add(userMarker);
            }
            else
            {
                userMarker.SetPos(position);
            }
        }

        public MapPosVector GetCirclePoints(double centerLon, double centerLat, double radius)
		{
            // Number of points of circle
            int N = 100;
			int EARTH_RADIUS = 6378137;

			MapPosVector points = new MapPosVector();

			for (int i = 0; i <= N; i++)
			{
				double angle = Math.PI * 2 * (i % N) / N;
				double dx = radius * Math.Cos(angle);
				double dy = radius * Math.Sin(angle);
				double lat = centerLat + (180 / Math.PI) * (dy / EARTH_RADIUS);
				double lon = centerLon + (180 / Math.PI) * (dx / EARTH_RADIUS) / Math.Cos(centerLat * Math.PI / 180);
                points.Add(Projection.FromWgs84(new MapPos(lon, lat)));
			}

			return points;
		}
    }

    public class VectorElementIgnoreListener : VectorElementEventListener
    {
        public override bool OnVectorElementClicked(VectorElementClickInfo clickInfo)
        {
            return false;
        }
    }
}
