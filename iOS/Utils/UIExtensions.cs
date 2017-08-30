
using System;

namespace data.collection.iOS
{
    public static class UIExtensions
    {
        public static UIKit.UIColor ToNativeColor(this Carto.Graphics.Color Color)
        {
            var red = (int)Color.R;
            var green = (int)Color.G;
            var blue = (int)Color.B;
            var alpha = (int)Color.A;

            var color = UIKit.UIColor.FromRGBA(red, green, blue, alpha);

            return color;
        }
    }
}
