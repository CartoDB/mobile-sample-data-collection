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
            builder.TitleFontSize = 12;
            builder.DescriptionFontSize = 10;

            var navy = new Color(22, 41, 69, 255);
            builder.TitleColor = navy;
            builder.DescriptionColor = navy;

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

            return true;
        }
    }
}
