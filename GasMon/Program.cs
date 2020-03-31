using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Threading.Tasks;
using Amazon;
using Amazon.Internal;
using Amazon.Runtime;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace GasMon
{

    class Program
    {
        private const string bucketName = "gasmonitoring-locationss3bucket-pgef0qqmgwba";
        private const string keyName = "locations.json";
        // Specify your bucket region (an example region is shown).
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUWest2;
        private static IAmazonS3 s3client = new AmazonS3Client(bucketRegion);
        private static AmazonSQSClient sqsclient = new AmazonSQSClient(bucketRegion);

        public static void Main()
        {
            // var locationsFetcher = new LocationsFetcher(s3client);
            // var locations = locationsFetcher.GetLocations(bucketName, keyName);
            // Console.WriteLine(locations);
            
            CreateQueueRequest createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = "GasMonQueue";
            CreateQueueResponse createQueueResponse =
                sqsclient.CreateQueueAsync(createQueueRequest).Result;
            string queueURL = createQueueResponse.QueueUrl;

            DeleteQueueRequest deleteQueueRequest = new DeleteQueueRequest(queueURL);
            sqsclient.DeleteQueueAsync(deleteQueueRequest);



        }
    }
}
