using System;
using Android.Graphics;

namespace data.collection.Droid
{
    public static class Colors
    {
        public static readonly Color DarkTransparentGray = Color.Argb(200, 50, 50, 50);

        public static readonly Color AppleBlue = Color.Argb(150, 0, 122, 255);

        public static readonly Color TransparentAppleBlue = Color.Argb(150, 0, 122, 255);

        public static readonly Color DarkTransparentAppleBlue = Color.Argb(200, 0, 122, 255);

        public static readonly Color CartoNavy = Color.Rgb(22, 41, 69);

        public static Color ToNativeColor(this Carto.Graphics.Color color)
        {
            return Color.Argb(color.A, color.R, color.G, color.B);
        }
    }
}
