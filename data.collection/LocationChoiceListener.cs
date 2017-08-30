
using System;
using System.Collections.Generic;
using Carto.Core;
using Carto.DataSources;
using Carto.Geometry;
using Carto.Graphics;
using Carto.Layers;
using Carto.Projections;
using Carto.Services;
using Carto.Styles;
using Carto.Ui;
using Carto.VectorElements;

namespace data.collection
{
    public class LocationChoiceListener : MapEventListener
    {
        public EventHandler<EventArgs> PinAdded;
        public EventHandler<EventArgs> QueryFailed;
        public EventHandler<EventArgs> PointsAdded;

        public MapView MapView { get; private set; }

        public Bitmap Bitmap { get; set; }

        LocalVectorDataSource markerSource;
        LocalVectorDataSource pointSource;

        public Projection Projection
        {
            get { return MapView.Options.BaseProjection; }
        }

        public MapPos MarkerPosition { get; set; }

        CartoSQLService Service;

        public LocationChoiceListener(MapView mapView)
        {
            MapView = mapView;

            markerSource = new LocalVectorDataSource(Projection);
            VectorLayer markerLayer = new VectorLayer(markerSource);
			MapView.Layers.Add(markerLayer);

            pointSource = new LocalVectorDataSource(Projection);
			VectorLayer pointLayer = new VectorLayer(pointSource);
			MapView.Layers.Add(pointLayer);

            Service = new CartoSQLService();
            Service.Username = "nutiteq";
		}

        public override void OnMapClicked(MapClickInfo mapClickInfo)
        {
            markerSource.Clear();

            MarkerPosition = mapClickInfo.ClickPos;
            var marker = GetUserMarker(Bitmap, MarkerPosition);
            markerSource.Add(marker);

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

		// Location Red
        public static readonly Color SyncedLocations = new Color(215, 82, 75, 255);
        // Carto Green
        public static readonly Color MySyncedLocations = new Color(145, 198, 112, 255);
		// Gray
        public static readonly Color UnsyncedLocations = new Color(99, 109, 114, 255);

        public void QueryPoints(string deviceId)
        {
            var builder = new PointStyleBuilder { Size = 12 };

            Dictionary<string, Color> ids = new Dictionary<string, Color>();

            System.Threading.Tasks.Task.Run(delegate
            {
                var query = "SELECT * FROM " + Codec.TableName;

                FeatureCollection features = null;
                try
                {
                    features = Service.QueryFeatures(query, Projection);
                }
                catch (Exception e)
                {
                    QueryFailed?.Invoke(e.Message, EventArgs.Empty);
                    return;
                }

                VectorElementVector points = new VectorElementVector();

                for (int i = 0; i < features.FeatureCount; i++)
                {
                    Feature feature = features.GetFeature(i);

                    PointGeometry geometry = (PointGeometry)feature.Geometry;

                    string id = feature.Properties.GetObjectElement(Data.DEVICEID).String;

                    if (id.Equals(deviceId))
                    {
                        builder.Color = MySyncedLocations;
                    }
                    else
                    {
                        builder.Color = SyncedLocations;
                    }

                    var point = new Point(geometry, builder.BuildStyle());
                    points.Add(point);
                }

                List<Data> unsynced = SQLClient.Instance.GetAll();

                foreach (var item in unsynced)
                {
                    MapPos position = item.GetPosition(Projection);
                    var geomery = new PointGeometry(position);

                    builder.Color = UnsyncedLocations;

                    var point = new Point(position, builder.BuildStyle());
                    points.Add(point);
                }

				pointSource.AddAll(points);

				PointsAdded?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}
