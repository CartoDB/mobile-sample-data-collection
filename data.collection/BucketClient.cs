using System;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using Amazon;
using System.Threading.Tasks;
using Carto.Core;

namespace data.collection
{
    public static class BucketClient
    {
        public static string Name = "com.carto.mobile.images";

        public static string AccessKey = "<your-s3-access-key>";
        public static string SecretKey = "<your-s3-secret-key>";

        public static string UploadPath = "https://" + Name + ".s3.amazonaws.com/";

        /* This is the base path, file name needs to be appended to this path, 
		 * e.g. https://s3.amazonaws.com/com.carto.mobile.images/test.jpg
         */
        public static string PublicReadPath = "https://s3.amazonaws.com/" + Name + "/";

        static IAmazonS3 client;

        public static async Task<BucketResponse> Upload(string filename, Stream stream)
        {
            BucketResponse response = new BucketResponse();

            PutObjectRequest request = new PutObjectRequest
            {
                BucketName = Name,
                Key = filename,
                CannedACL = S3CannedACL.PublicRead,
                InputStream = stream
            };

            PutObjectResponse intermediary = new PutObjectResponse();

            using (client = new AmazonS3Client(AccessKey, SecretKey, RegionEndpoint.USEast1))
            {
                try
                {
                    intermediary = await client.PutObjectAsync(request);
                    response.Message = "Image uploaded";
                    response.Path = PublicReadPath + filename;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                    response.Error = e.Message;
                }
            }

            return response;
        }


        public static void Initialize(
#if __ANDROID__
            Android.Content.Res.AssetManager assets
#endif
        )
        {
            string name = "s3_tokens.txt";
#if __ANDROID__
            var stream = assets.Open(name);
            var text = new StreamReader(stream).ReadToEnd();
#elif __IOS__
            var text = File.ReadAllText(name);
#endif
			Variant variant = Variant.FromString(text);

			AccessKey = variant.GetObjectElement("access_key").String;
			SecretKey = variant.GetObjectElement("secret_key").String;
		}
	}
}

