
using System;
using System.Timers;
using CoreGraphics;
using UIKit;

namespace data.collection.iOS
{
    public class Banner : UIView
    {
        UIImageView imageView;
        UIActivityIndicatorView spinner;

        UILabel label;

        Timer completeTimer;

        void SetText(string text, bool autoclose)
        {
			label.Text = text;
			Show();

            if (!autoclose)
            {
                return;
            }

			if (completeTimer != null)
			{
				completeTimer.Stop();
				completeTimer.Dispose();
				completeTimer = null;
			}

            completeTimer = new Timer { Interval = 3000 };
            completeTimer.Start();

			completeTimer.Elapsed += delegate
			{
				InvokeOnMainThread(delegate
				{
					Hide();
				});

				completeTimer.Stop();
				completeTimer.Dispose();
				completeTimer = null;
			};
        }

        public Banner()
        {
            BackgroundColor = Colors.DarkTransparentGray;

            imageView = new UIImageView();
            imageView.ContentMode = UIViewContentMode.ScaleAspectFill;
            imageView.ClipsToBounds = true;
            imageView.Image = UIImage.FromFile("icon_banner_info.png");
            AddSubview(imageView);

            spinner = new UIActivityIndicatorView();
            spinner.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.White;
            AddSubview(spinner);

            label = new UILabel();
            label.Font = UIFont.FromName("HelveticaNeue", 12);
            label.TextAlignment = UITextAlignment.Center;
            label.TextColor = UIColor.White;
            AddSubview(label);

            Alpha = 0.0f;
        }

        public override void LayoutSubviews()
        {
            nfloat padding = Frame.Height / 4;

            nfloat x = padding;
            nfloat y = padding;
            nfloat w = Frame.Height - 2 * padding;
            nfloat h = w;

            imageView.Frame = new CGRect(x, y, w, h);

            spinner.Frame = new CGRect(x, y, w, h);

            x += w + padding;
            y = 0;
            w = Frame.Width - (2 * x);
            h = Frame.Height;

            label.Frame = new CGRect(x, y, w, h);
        }

        public void Show()
        {
            Superview.BringSubviewToFront(this);
            AnimateAlpha(1.0f);
        }

        public void Hide()
        {
            AnimateAlpha(0.0f);
        }

        void AnimateAlpha(nfloat to)
        {
            Animate(0.3, delegate {
                Alpha = to;   
            });
        }

        public void SetInformationText(string text, bool autoclose)
        {
            SetText(text, autoclose);
            ShowInfo();
        }

        public void ShowUploadingImage()
        {
            SetText("Uploading image to amazon...", false);
            ShowSpinner();
        }

		public void ShowUploadingData()
		{
            SetText("Uploading data to CARTO...", false);
            ShowSpinner();
		}

		public void Complete()
		{
            SetText("Great success! Data uploaded", true);
            ShowInfo();
		}

        public void ShowFailedAmazonUpload()
        {
            SetText("Failed to upload image. Saving data locally", true);
            ShowInfo();
        }

		public void ShowFailedCartoUpload()
		{
			SetText("Failed to upload data. Making a local copy", true);
			ShowInfo();
		}

        public void ShowUploadingEverything(int count)
        {
            SetText("Uploading " + count + GetCounter(count), false);
            ShowSpinner();
        }

		public void ShowUploadedEverything(int count)
		{
			SetText("Great success! Uploaded " + count + GetCounter(count), true);
			ShowInfo();
		}
		
        public void ShowEverythingUploadFailed(int count)
		{
            SetText("Failed to upload " + count + GetCounter(count) + "Are you sure you're online?", true);
            ShowInfo();
		}

        string GetCounter(int count)
        {
            if (count == 1)
            {
                return " item...";
            }

            return " items...";
        }

        void ShowSpinner()
        {
            spinner.Hidden = false;
            spinner.StartAnimating();
            imageView.Hidden = true;
        }

        void ShowInfo()
        {
            spinner.Hidden = true;
            spinner.StopAnimating();
            imageView.Hidden = false;
        }
    }
}
