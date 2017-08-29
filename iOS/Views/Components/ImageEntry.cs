
using System;
using System.Linq;
using Carto.Core;
using Carto.Layers;
using Carto.Ui;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class ImageEntry : BaseEntry
    {
        public string ImageName { get; set; }

        public UIImageView Photo { get; private set; }

        UIImageView image;

        public ImageEntry(string title, string resource) : base(title)
        {
            Photo = new UIImageView();
			Photo.ClipsToBounds = true;
            Photo.ContentMode = UIViewContentMode.ScaleAspectFill;
            AddSubview(Photo);

            image = new UIImageView();
            image.Image = UIImage.FromFile(resource);
			image.ClipsToBounds = true;
			image.ContentMode = UIViewContentMode.ScaleAspectFit;
            AddSubview(image);

            BringSubviewToFront(label);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            Photo.Frame = Bounds;

            nfloat w = Frame.Height / 3;
            nfloat h = w;
            nfloat x = Frame.Width / 2 - w / 2;
            nfloat y = Frame.Height / 2 - h / 2;

			image.Frame = new CGRect(x, y, w, h);
        }

        UITapGestureRecognizer recognizer;

        public void AddGestureRecognizer(Action action)
        {
            recognizer = new UITapGestureRecognizer(action);
            AddGestureRecognizer(recognizer);   
        }

        public void RemoveGestureRecognizer()
        {
            RemoveGestureRecognizer(recognizer);
        }

        public bool IsSet
        {
            get
            {
                return Subviews.Any(view => view is MapView);
            }
        }

        public void AddMap(MapView mapView, MapPos position)
        {
            AddSubview(mapView);
            mapView.Frame = Bounds;

            BringSubviewToFront(image);

            mapView.Zoom = 18;
            mapView.FocusPos = position;

            var layer = new CartoOnlineVectorTileLayer(CartoBaseMapStyle.CartoBasemapStyleVoyager);
            mapView.Layers.Add(layer);
        }
    }
}
