
using System;
using System.IO;
using Carto.Core;
using Carto.Projections;

namespace data.collection
{
    public class Data
    {
        public const string DEVICEID = "device_identifier";

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

        public double Time { get; set; }

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
            map.Add("title", new Variant(Title));
            map.Add("description", new Variant(Description));
            map.Add("attachment_url", new Variant(ImageUrl));
            map.Add("marker_latitude", new Variant(MarkerLatitude));
            map.Add("marker_longitude", new Variant(MarkerLongitude));
            map.Add(DEVICEID, new Variant(Identifier));
			map.Add("user_longitude", new Variant(Longitude));
			map.Add("user_latitude", new Variant(Latitude));
			map.Add("user_accuracy", new Variant(Accuracy));
            map.Add("report_time", new Variant(Time));

            string result = new Variant(map).ToString();

            return result;
        }

		public MapPos GetPosition(Projection projection)
		{
			return projection.FromWgs84(new MapPos(Longitude, Latitude));
		}

		public static Data Get(string id, string url, string title, string description)
		{
			var item = new Data();
            item.Identifier = id;

			item.ImageUrl = url;
            item.Title = title;
            item.Description = description;

			item.Latitude = LocationClient.Latitude;
			item.Longitude = LocationClient.Longitude;
			item.Accuracy = LocationClient.Accuracy;

			item.MarkerLatitude = LocationClient.MarkerLatitude;
			item.MarkerLongitude = LocationClient.MarkerLongitude;

			item.Time = DateUtils.CurrentMilliseconds;

			return item;
		}

    }
}
