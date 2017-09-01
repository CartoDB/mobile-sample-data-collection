using System;
using System.IO;
#if __ANDROID__
using SQLite.Net;
using SQLite.Net.Interop;
#elif __IOS__
using SQLite;
#endif
using System.Collections.Generic;

namespace data.collection
{
    public class SQLClient
    {
        public static readonly SQLClient Instance = new SQLClient();
        readonly SQLiteConnection db;
		const string file = "data.collection.db";
        SQLClient()
        {
            string folder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = Path.Combine(folder, file);
#if __ANDROID__
            Directory.CreateDirectory(folder);
			SQLite.SQLite3.Config(SQLite.SQLite3.ConfigOption.Serialized);

            var platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroidN();
            db = new SQLiteConnection(platform, folder, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex);

#elif __IOS__
            db = new SQLiteConnection(path);
#endif
			// 'Create' ensures that it's created, does not recreate if exists
			db.CreateTable<Data>();
        }

        public void Insert(Data data)
        {
            db.Insert(data);
        }

        public List<Data> GetAll()
        {
            return db.Query<Data>("SELECT * FROM Data");
        }

        public void Delete(Data data)
        {
            db.Delete(data);
        }

        public void DeleteAll()
        {
            db.DeleteAll<Data>();
        }

        public void UpdateAll(List<Data> items)
        {
            db.UpdateAll(items);
        }
    }
}
