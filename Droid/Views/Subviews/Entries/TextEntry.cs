
using System;
using Android.Widget;
using Android.Content;
using Android.Graphics;
using Android.Views.InputMethods;

namespace data.collection.Droid
{
    public class TextEntry : BaseEntry
    {
        public EditText Field { get; private set; }

		public string Text
		{
			get { return Field.Text; }
			set { Field.Text = value; }
		}

        public ImeAction ImeOptions
        {
            get { return Field.ImeOptions; }
            set { Field.ImeOptions = value; }
        }


        public TextEntry(Context context, string title, bool isRequired = false) : base(context, title, isRequired)
        {
            Field = new EditText(context);
            Field.TextSize = 14.0f;
            Field.SetTextColor(Color.DarkGray);
            Field.SetBackground(Color.White);
            Field.ImeOptions = Android.Views.InputMethods.ImeAction.Done;
            Field.InputType = Android.Text.InputTypes.TextFlagImeMultiLine;

            Field.SetBackground(Color.White);
            Field.SetBorderColor((int)(1 * Density), Colors.CartoNavy);

            AddView(Field);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

			int x = padding;
            int y = label.MeasuredHeight + 2 * padding;
            int w = Frame.W - 2 * padding;
            int h = Frame.H - (label.MeasuredHeight + 3 * padding);

            Field.SetFrame(x, y, w, h);
        }
    }
}
