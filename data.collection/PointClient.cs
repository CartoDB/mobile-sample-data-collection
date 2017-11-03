
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
    public class PointClient
    {
        public EventHandler<EventArgs> QueryFailed;
        public EventHandler<EventArgs> PointsAdded;

        public MapView MapView { get; private set; }

        public Bitmap Bitmap { get; set; }

        public LocalVectorDataSource MarkerSource { get; private set; }

        public Projection Projection
        {
            get { return MapView.Options.BaseProjection; }
        }

        public MapPos MarkerPosition { get; set; }

        public LocalVectorDataSource PopupSource { get; private set; }
        VectorLayer popupLayer;

        public ElementClickListener Listener { get; private set; }

        public PointClient(MapView mapView)
        {
            MapView = mapView;

            MarkerSource = new LocalVectorDataSource(Projection);
            VectorLayer markerLayer = new VectorLayer(MarkerSource);
			MapView.Layers.Add(markerLayer);

            PopupSource = new LocalVectorDataSource(Projection);
            popupLayer = new VectorLayer(PopupSource);
            mapView.Layers.Add(popupLayer);

            Listener = new ElementClickListener(PopupSource);
		}

        public void AddUserMarker(MapPos position)
        {
            var builder = new MarkerStyleBuilder();
            builder.Bitmap = Bitmap;
            builder.HideIfOverlapped = false;
            builder.Size = 30;
            builder.Color = new Color(0, 255, 0, 255);

            var animationBuilder = new AnimationStyleBuilder();
            animationBuilder.SizeAnimationType = AnimationType.AnimationTypeSpring;
            builder.AnimationStyle = animationBuilder.BuildStyle();

            MarkerSource.Add(new Marker(position, builder.BuildStyle()));
        }

		// Location Red
        public static readonly Color SyncedLocations = new Color(215, 82, 75, 255);
        // Carto Green
        public static readonly Color MySyncedLocations = new Color(145, 198, 112, 255);
		// Gray
        public static readonly Color UnsyncedLocations = new Color(99, 109, 114, 255);

        VectorTileLayer pointLayer;
        public void QueryPoints(Action complete)
        {
            if  (pointLayer != null)
            {
                pointLayer.VectorTileEventListener = null;
                MapView.Layers.Remove(pointLayer);

            }

            var username = "nutiteq";
            var mapname = "tpl_76370647_9649_4d19_a6d5_4144348a6f67";

            MapView.ConfigureNamedVectorLayers(username, mapname, (VectorTileLayer obj) =>
            {
                complete();
                pointLayer = obj;
                MapView.Layers.Add(pointLayer);

                pointLayer.VectorTileEventListener = Listener;
            });
        }
    }
}
