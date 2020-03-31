using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Internal;
using Amazon.Runtime;

namespace GasMon
{

    class Program
    {
        private const string bucketName = "gasmonitoring-locationss3bucket-pgef0qqmgwba";
        private const string keyName = "locations.json";
        // Specify your bucket region (an example region is shown).
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUWest2;
        private static IAmazonS3 client = new AmazonS3Client(bucketRegion);

        public static void Main()
        {
            var locationsFetcher = new LocationsFetcher(client);
            var locations = locationsFetcher.GetLocations(bucketName, keyName);
            Console.WriteLine(locations);
        }
    }
}
