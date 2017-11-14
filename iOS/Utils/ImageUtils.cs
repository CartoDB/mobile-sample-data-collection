
using System;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class ImageUtils
    {
        public static UIImage Resize(UIImage source)
        {
            return MaxResize(source, 800, 800);
        }

        public static UIImage Resize(UIImage source, float size)
        {
            return MaxResize(source, size, size);
        }

		// resize the image to be contained within a maximum width and height, keeping aspect ratio
        static UIImage MaxResize(UIImage sourceImage, float maxWidth, float maxHeight)
		{
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			
            if (maxResizeFactor > 1)
            {
                return sourceImage;   
            }

			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;
			
            UIGraphics.BeginImageContext(new CGSize(width, height));
            sourceImage.Draw(new CGRect(0, 0, width, height));
            var result = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();
			
            return result;
		}
    }
}
