using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Internal;
using Amazon.Runtime;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace GasMon
{
    class Program
    {
        private const string BucketName = "gasmonitoring-locationss3bucket-pgef0qqmgwba";
        private const string KeyName = "locations.json";
        private const string TopicArn =
            "arn:aws:sns:eu-west-2:099421490492:GasMonitoring-snsTopicSensorDataPart1-1YOM46HA51FB";
        private static readonly RegionEndpoint BucketRegion = RegionEndpoint.EUWest2;

        private static readonly IAmazonS3 S3Client = new AmazonS3Client(BucketRegion);
        private static readonly IAmazonSQS SqsClient = new AmazonSQSClient(BucketRegion);
        private static readonly IAmazonSimpleNotificationService SnsClient =
            new AmazonSimpleNotificationServiceClient(BucketRegion);

        private const int RunTime = 20;
        private const int WaitTime = 5;
        
        public static void Main()
        {
            var locationsFetcher = new LocationsFetcher(S3Client);
            var locations = locationsFetcher.GetLocations(BucketName, KeyName);
            var locationIds = locations.Select(l => l.Id).ToList();
            var processor = new MessageProcessor(locationIds);


            

            

            using (var queue = new SubscribedQueue(SqsClient, SnsClient, TopicArn))
            {
                var timeNow = DateTime.Now;
                var endTime = timeNow.AddSeconds(RunTime);
                Console.WriteLine("Processing messages....");

                while (DateTime.Now < endTime)
                {
                    var messageResponse = processor.CollectMessages(queue, SqsClient);
                    processor.ProcessMessagesFromResponse(messageResponse);
                }
                
                Console.WriteLine("Readings with removals: " + processor.Readings.Count);
                Console.WriteLine("finished processing messages.");
            }
        }
    }
}