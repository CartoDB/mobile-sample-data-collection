using System;
using System.IO;
using Foundation;
using UIKit;

namespace data.collection.iOS
{
    public class Camera
    {
        public const string JPG = ".jpg";
        public const string PNG = ".png";

        public static string Extension = JPG;
        public static readonly Camera Instance = new Camera();

        static UIImagePickerController picker;

        public static string LatestImageName { get; private set; }

        public static string LatestImagePath
        {
            get { return FileUtils.GetFolder(LatestImageName); }
        }

        public CameraDelegate Delegate;

        Camera()
        {
            picker = new UIImagePickerController();

            Delegate = new CameraDelegate();
            picker.Delegate = Delegate;
        }

        public void TakePicture(UIViewController parent)
        {
            picker.SourceType = UIImagePickerControllerSourceType.Camera;
            parent.PresentViewController(picker, true, delegate
            {
                Console.WriteLine("Camera Opened");
            });
        }

        public void SelectPicture(UIViewController parent)
        {
            picker.SourceType = UIImagePickerControllerSourceType.PhotoLibrary;
            parent.PresentViewController(picker, true, delegate
            {
                Console.WriteLine("Photo Libary Opened");
            });
        }

        public static UIImage GetImageFromInfo(NSDictionary info)
        {
            return info.ValueForKey(new NSString("UIImagePickerControllerOriginalImage")) as UIImage;
        }

        public static Stream GetStreamFromImage(UIImage image)
        {
            NSData item = GetImageData(image);
            return item.AsStream();
        }

        static NSData GetImageData(UIImage image)
        {
			if (Extension.Equals(PNG))
			{
				return image.AsPNG();
			}
			
            return image.AsJPEG();
		
        }

        public static void SaveImage(UIImage image, string name)
        {
            LatestImageName = name;
            string filename = FileUtils.GetFolder(LatestImageName);

            NSData item = GetImageData(image);

			NSError err = null;

            if (item.Save(filename, false, out err))
			{
				Console.WriteLine("Saved as " + filename);
			}
			else
			{
				Console.WriteLine("NOT saved as " + filename + " because " + err.LocalizedDescription);
			}
        }
    }

    public class CameraDelegate : UIImagePickerControllerDelegate
    {
		public EventHandler<PhotoEventArgs> Complete;

        public override void FinishedPickingMedia(UIImagePickerController picker, NSDictionary info)
        {
            picker.DismissViewController(true, delegate
            {
                Complete?.Invoke(picker, new PhotoEventArgs { Info = info });
            });
        }
    }

    public class PhotoEventArgs : EventArgs
    {
        public NSDictionary Info { get; set; }
    }

}
