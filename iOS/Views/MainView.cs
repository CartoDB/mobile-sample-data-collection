﻿
using System;
using Carto.Core;
using Carto.Layers;
using Carto.Projections;
using Carto.Ui;
using CoreGraphics;
using Foundation;
using UIKit;

namespace data.collection.iOS
{
    public class MainView : BaseView, IUITextViewDelegate
    {
        public TextEntry TitleField { get; private set; }

        public TextEntry DescriptionField { get; private set; }

        public ImageEntry CameraField { get; private set; }

        public ImageEntry LocationField { get; private set; }

        public SubmitButton Submit { get; private set; }

		public MainView()
        {
            BackgroundColor = UIColor.FromRGB(245, 245, 245);

            TitleField = new TextEntry("TITLE");
            TitleField.Field.Delegate = this;
            AddSubview(TitleField);

            DescriptionField = new TextEntry("DESCRIPTION");
            DescriptionField.Field.Delegate = this;
            AddSubview(DescriptionField);

            CameraField = new ImageEntry("TAKE PHOTO", "icon_camera.png");
            AddSubview(CameraField);

			LocationField = new ImageEntry("ADD LOCATION", "icon_add_location.png");
			AddSubview(LocationField);

            Submit = new SubmitButton();
            AddSubview(Submit);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            nfloat padding = 15;

            nfloat x = padding;
            nfloat y = Device.TrueY0 + padding;
			nfloat w = Frame.Width - 2 * padding;
            nfloat h = 60;

            TitleField.Frame = new CGRect(x, y, w, h);

            y += h + padding;
            h = 150;

            DescriptionField.Frame = new CGRect(x, y, w, h);

            y += h + padding;

            w = (Frame.Width - 3 * padding) / 2;

            CameraField.Frame = new CGRect(x, y, w, h);

            x += w + padding;

            LocationField.Frame = new CGRect(x, y, w, h);

            y += h + padding;

            x = padding;
            w = Frame.Width - 2 * padding;
            h = 50;

            Submit.Frame = new CGRect(x, y, w, h);
        }

        public bool IsAnyFieldEmpty
        {
            get 
            {
                if (string.IsNullOrWhiteSpace(TitleField.Text))
                {
                    return true;
                }

                if (string.IsNullOrWhiteSpace(DescriptionField.Text))
				{
					return true;
				}

                if (CameraField.Photo.Image == null)
                {
                    return true;
                }

                if (!LocationField.IsSet)
                {
                    return true;
                }

                return false;
            }
        }

        MapView mapView;
        MapView MapView
        {
            get
            {
                if (mapView == null)
                {
                    mapView = new MapView();
                }

                return mapView;
            }
        }

        Projection Projection => MapView.Options.BaseProjection;

        public void AddMapOverlayTo(double longitude, double latitude)
        {
            MapPos position = Projection.FromWgs84(new MapPos(longitude, latitude));

            Console.WriteLine(position);

            LocationField.AddMap(MapView, position);
        }

        const string NewLine = "\n";

        [Export("textView:shouldChangeTextInRange:replacementText:")]
        public bool ShouldChangeText(UITextView textView, NSRange range, string text)
        {
            if (text.Equals(NewLine))
            {
				DescriptionField.Field.ResignFirstResponder();
				TitleField.Field.ResignFirstResponder();        
            }

            return true;
        }

    }
}
