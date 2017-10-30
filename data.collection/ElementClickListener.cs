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
    public class ElementClickListener : VectorElementEventListener
    {
        public const string NullId = "null";
        public const string TitleId = "_title";
        public const string DescriptionId = "_description";

        LocalVectorDataSource source;

        BalloonPopup previous;

        public ElementClickListener(LocalVectorDataSource dataSource)
        {
            source = dataSource;
        }

        public override bool OnVectorElementClicked(VectorElementClickInfo clickInfo)
        {
            if (previous != null)
            {
                source.Remove(previous);
            }

            VectorElement element = clickInfo.VectorElement;

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

            string title = element.GetMetaDataElement(TitleId).String;
            string description = element.GetMetaDataElement(DescriptionId).String;

            BalloonPopup popup;

            if (element is BalloonPopup)
            {
                Billboard billboard = (Billboard)element;
                popup = new BalloonPopup(billboard, style, title, description);
            }
            else
            {
                MapPos position = clickInfo.ClickPos;
                popup = new BalloonPopup(position, style, title, description);
            }

            source.Add(popup);
            previous = popup;

            return true;
        }
    }
}
