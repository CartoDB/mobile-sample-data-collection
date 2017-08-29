
using System;
using System.IO;
using Carto.Core;

namespace data.collection
{
    public class Data
    {
        [SQLite.PrimaryKey, SQLite.AutoIncrement]
        public int Id { get; set; }

        public string Identifier { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public double Accuracy { get; set; }

        public double MarkerLatitude { get; set; }

        public double MarkerLongitude { get; set; }

        public bool IsUploadedToAmazon
        {
            get { return ImageUrl.Contains(BucketClient.PublicReadPath); }
        }

        public string FileName
        {
            get { return Path.GetFileName(ImageUrl); }
        }

        public override string ToString()
        {
            var map = new StringVariantMap();
            map.Add("identifier", new Variant(Identifier));
            map.Add("title", new Variant(Title));
            map.Add("description", new Variant(Description));
            map.Add("image_url", new Variant(ImageUrl));
            map.Add("user_longitude", new Variant(Longitude));
            map.Add("user_latitude", new Variant(Latitude));
            map.Add("user_accuracy", new Variant(Accuracy));
            map.Add("marker_latitude", new Variant(MarkerLatitude));
            map.Add("marker_longitude", new Variant(MarkerLongitude));

            string result = new Variant(map).ToString();

            return result;
        }
    }
}
