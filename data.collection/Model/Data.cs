
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
            map.Add("longitude", new Variant(Longitude));
            map.Add("latitude", new Variant(Latitude));
            map.Add("accuracy", new Variant(Accuracy));

            string result = new Variant(map).ToString();

            return result;
        }
    }
}
