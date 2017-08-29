using System;
using System.IO;

namespace data.collection
{
    public class FileUtils
    {
		public static string GetFolder(string name)
		{
			var folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			return Path.Combine(folder, name);
		}

	}
}
