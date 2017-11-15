using System;
namespace data.collection
{
    /*
     * Conf.cs template file. 
     * You should use this template when creating your own Conf.cs file, 
     * but be sure to replace existing placeholder values.
     */
    public class Conf_Template
    {
        // This is your CARTO account name.
        // User name is used in CartoMobileSDK's Maps API,
        public const string Username = "<your-username>";

        // Schema is the same as your CARTO user name,
        // separating it into a separate variable for clarity.
        public const string Schema = Username;

        // The function you've crated in CARTO Engine (via CDB or otherwise)
        public const string FunctionName = "<your-function-name>";

        // The map you've created in CARTO Builder
        public const string MapName = "tpl_76370647_9649_4d19_a6d5_4144348a6f67";

        // Your AWS Bucket name
        public const string S3BucketName = "<your-bucket-name";

        // AWS S3 Bucket authentication.
        // Make sure your bucket has public access,
        // since GET queries do not contain these keys
        public const string S3AccessKey = "<your-access-key>";
        public const string S3SecretKey = "<your-secret-key>";
    }
}
