using System;
using Carto.DataSources;
using Carto.Layers;
using Carto.Styles;
using Carto.Ui;
using Carto.VectorElements;
using Carto.Graphics;
using System.Linq;
using Carto.Core;

namespace data.collection
{
    public class ElementClickListener : VectorTileEventListener
    {
        public EventHandler<EventArgs> Click;

        public Bitmap LeftImage { get; set; }

        LocalVectorDataSource source;

        public BalloonPopup Previous { get; private set; }

        public ElementClickListener(LocalVectorDataSource source)
        {
            this.source = source;
        }

        public override bool OnVectorTileClicked(VectorTileClickInfo clickInfo)
        {
            if (Previous != null)
            {
                source.Remove(Previous);
            }

            var feature = clickInfo.Feature;
            var properties = feature.Properties;

            BalloonPopupStyleBuilder builder = new BalloonPopupStyleBuilder();
            builder.LeftMargins = new BalloonPopupMargins(0, 0, 0, 0);
            builder.RightMargins = new BalloonPopupMargins(6, 3, 6, 3);
            builder.PlacementPriority = 10;
            builder.CornerRadius = 5;

            builder.TitleFontSize = 14;
            builder.TitleColor = new Color(20, 20, 20, 255);
            builder.DescriptionFontSize = 9;
            builder.DescriptionColor = new Color(100, 100, 100, 255);

            builder.LeftMargins = new BalloonPopupMargins(6, 6, 6, 6);
            builder.LeftImage = LeftImage;
            builder.RightMargins = new BalloonPopupMargins(2, 6, 12, 6);

            var animationBuilder = new AnimationStyleBuilder();
            animationBuilder.RelativeSpeed = 2.0f;
            animationBuilder.SizeAnimationType = AnimationType.AnimationTypeSpring;
            builder.AnimationStyle = animationBuilder.BuildStyle();

            BalloonPopupStyle style = builder.BuildStyle();

            string title = properties.GetObjectElement("title").String;
            string description = properties.GetObjectElement("description").String;

            MapPos position = clickInfo.ClickPos;
            BalloonPopup popup = new BalloonPopup(position, style, title, description);;

            source.Add(popup);
            Previous = popup;

            string url = properties.GetObjectElement("attachment_url").String;
            Click?.Invoke(url, EventArgs.Empty);

            return true;
        }
    }
}
