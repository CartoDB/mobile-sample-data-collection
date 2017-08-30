
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

        LocalVectorDataSource source;

        public Projection Projection
        {
            get { return MapView.Options.BaseProjection; }
        }

        public MapPos MarkerPosition { get; set; }

        CartoSQLService Service;

        public LocationChoiceListener(MapView mapView)
        {
            MapView = mapView;

			source = new LocalVectorDataSource(Projection);
            VectorLayer routeLayer = new VectorLayer(source);
			MapView.Layers.Add(routeLayer);

            Service = new CartoSQLService();
            Service.Username = "nutiteq";
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

        public void QueryPoints()
        {
            var builder = new PointStyleBuilder
            {
                Size = 10,
                Color = new Color(255, 0, 0, 255)
            };

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

                    var point = new Point(geometry, builder.BuildStyle());
                    points.Add(point);
                }

                source.AddAll(points);

                PointsAdded?.Invoke(this, EventArgs.Empty);
            });
        }
    }
}
