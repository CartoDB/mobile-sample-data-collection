
using System;

namespace data.collection.Droid
{
    public class CGRect
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public int W { get; private set; }

        public int H { get; private set; }

        public int Bottom { get { return Y + H; } }

        public int Right { get { return W + X; } }

        public CGRect(int x, int y, int w, int h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }

        public static CGRect Empty { get { return new CGRect(0, 0, 0, 0); } }


    }
}
