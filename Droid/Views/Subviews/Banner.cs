using System;
using System.Timers;
using Android.Animation;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Widget;

namespace data.collection.Droid
{
    public class Banner : BaseView
    {
        readonly TextView label;
        readonly ImageView imageView;
        readonly ProgressBar spinner;

        public Banner(Context context) : base(context)
        {
            this.SetBackground(Colors.DarkTransparentGray);

            label = new TextView(context);
            label.Gravity = Android.Views.GravityFlags.Center;
            label.TextSize = 13.0f;
            label.SetTextColor(Color.White);
            AddView(label);

            imageView = new ImageView(context);
            imageView.SetImageResource(Resource.Drawable.icon_banner_info);
			imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            AddView(imageView);

            spinner = new ProgressBar(context);
            AddView(spinner);

            Alpha = 0.0f;
        }

        public override void LayoutSubviews()
        {
            int padding = Frame.H / 4;

			int x = padding;
			int y = padding;
            int w = Frame.H - 2 * padding;
			int h = w;

            imageView.SetFrame(x, y, w, h);

            spinner.SetFrame(x, y, w, h);

			x += w + padding;
			y = 0;
            w = Frame.W - (2 * x);
            h = Frame.H;

            label.SetFrame(x, y, w, h);
        }

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
                (Context as Activity).RunOnUiThread(delegate
				{
					Hide();
				});

				completeTimer.Stop();
				completeTimer.Dispose();
				completeTimer = null;
			};
		}

		public void SetInformationText(string text, bool autoclose)
		{
			SetText(text, autoclose);
			ShowInfo();
		}

		public void SetLoadingText(string text, bool autoclose)
		{
			SetText(text, autoclose);
			ShowSpinner();
		}

		public void Show()
        {
            AnimateAlpha(1.0f);
        }

        public void Hide()
        {
            AnimateAlpha(0.0f);
        }

		void AnimateAlpha(float to, long duration = 300)
		{
			var animator = ObjectAnimator.OfFloat(this, "Alpha", to);
			animator.SetDuration(duration);
			animator.Start();
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
            spinner.Visibility = Android.Views.ViewStates.Visible;
            imageView.Visibility = Android.Views.ViewStates.Gone;
		}

		void ShowInfo()
		{
            spinner.Visibility = Android.Views.ViewStates.Gone;
            imageView.Visibility = Android.Views.ViewStates.Visible;
		}
    }
}
