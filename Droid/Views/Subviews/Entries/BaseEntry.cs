
using System;
using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Widget;

namespace data.collection.Droid
{
    public class BaseEntry : BaseView
    {
        protected TextView label;

        public bool IsRequired { get; private set; }

        public BaseEntry(Context context, string title, bool isRequired = false) : base(context)
        {
            label = new TextView(context);
            label.SetTextColor(Colors.CartoNavy);
            label.TextSize = 10;
            AddView(label);

            IsRequired = isRequired;

            if (isRequired)
            {
                var span = new SpannableString(title + " *");
                span.SetSpan(new ForegroundColorSpan(Colors.CartoNavy), 0, title.Length, SpanTypes.ExclusiveExclusive);
                span.SetSpan(new ForegroundColorSpan(Color.Red), title.Length, title.Length + 2, SpanTypes.ExclusiveExclusive);
                span.SetSpan(new StyleSpan(TypefaceStyle.Bold), title.Length, title.Length + 2, SpanTypes.ExclusiveExclusive);
                label.TextFormatted = span;
            }
            else
            {
                label.Text = title;
            }
        }

        protected int padding
        {
            get { return (int)(5 * Density); }
        }

        public override void LayoutSubviews()
        {
            label.Measure(0, 0);

			int x = padding;
			int y = padding;
            int w = Frame.W - 2 * padding;
            int h = label.MeasuredHeight;

            label.SetFrame(x, y, w, h);
        }
    }
}
