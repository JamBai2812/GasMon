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
        private const string bucketName = "gasmonitoring-locationss3bucket-pgef0qqmgwba";
        private const string keyName = "locations.json";

        private const string topicARN =
            "arn:aws:sns:eu-west-2:099421490492:GasMonitoring-snsTopicSensorDataPart1-1YOM46HA51FB";
        // Specify your bucket region (an example region is shown).
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.EUWest2;
        private static IAmazonS3 s3client = new AmazonS3Client(bucketRegion);
        private static AmazonSQSClient sqsclient = new AmazonSQSClient(bucketRegion);
        private static AmazonSimpleNotificationServiceClient snsclient = new AmazonSimpleNotificationServiceClient(bucketRegion);

        public static void Main()
        {
            var processor = new MessageProcessor();
            
            
            // var locationsFetcher = new LocationsFetcher(s3client);
            // var locations = locationsFetcher.GetLocations(bucketName, keyName);
            // Console.WriteLine(locations);
            
            // using (var queue = new SubscribedQueue(sqsclient, snsclient, topicARN))
            // {
            //     
            //     
            // }
            
            
            //Create Queue
            CreateQueueRequest createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = "JamieGasMonQueue";
            CreateQueueResponse createQueueResponse =
                sqsclient.CreateQueueAsync(createQueueRequest).Result;
            string queueUrl = createQueueResponse.QueueUrl;
            
            Console.WriteLine(queueUrl);
            
            //Subscribe queue to topic
            var subscriptionArn = snsclient.SubscribeQueueAsync(topicARN, sqsclient, queueUrl).Result;

            //Collect Messages
            var timeNow = DateTime.Now;
            var endTime = timeNow.AddSeconds(20);
            ReceiveMessageResponse result = new ReceiveMessageResponse();
            
            while (DateTime.Now < endTime)
            {
                
                var receiveMessageRequest = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    WaitTimeSeconds = 5
                };
                result = sqsclient.ReceiveMessageAsync(receiveMessageRequest).Result;
            }
            
            //Process Messages
            if (result.Messages.Count != 0)
            {
                foreach (var message in result.Messages)
                {
                    Console.WriteLine(message.Body);
                }
            }
            else
            {
                Console.WriteLine("Contains no messages");
            }
            
            //Also delete subscription

            snsclient.UnsubscribeAsync(subscriptionArn);


            //Delete Queue
            var deleteQueueRequest = new DeleteQueueRequest(queueUrl);
            sqsclient.DeleteQueueAsync(deleteQueueRequest);
            
            Console.WriteLine("finished.");
            
        }
    }
}
