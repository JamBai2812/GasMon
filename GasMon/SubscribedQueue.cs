using System;
using Amazon.SimpleNotificationService;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace GasMon
{
    public class SubscribedQueue : IDisposable
    {
        private readonly IAmazonSimpleNotificationService _snsClient;
        private readonly IAmazonSQS _sqsClient;
        private readonly string _subscriptionArn;
        public readonly string QueueUrl;
        
        
        public SubscribedQueue(IAmazonSQS sqsClient, IAmazonSimpleNotificationService snsClient, string topicARN)
        {
            _snsClient = snsClient;
            _sqsClient = sqsClient;
            
            
            //Create Queue
            CreateQueueRequest createQueueRequest = new CreateQueueRequest();
            createQueueRequest.QueueName = "JamieGasMonQueue";
            CreateQueueResponse createQueueResponse =
                _sqsClient.CreateQueueAsync(createQueueRequest).Result;
            QueueUrl = createQueueResponse.QueueUrl;
            Console.WriteLine("Queue created with URL:");
            Console.WriteLine(QueueUrl);
            
            //Subscribe queue to topic
            _subscriptionArn = _snsClient.SubscribeQueueAsync(topicARN, _sqsClient, QueueUrl).Result;
            Console.WriteLine("Subscribed to topic.");
        }
        
        
        public void Dispose()
        {
            //delete subscription
            _snsClient.UnsubscribeAsync(_subscriptionArn);
            Console.WriteLine("Unsubscribed from topic.");
            
            //Delete Queue
            var deleteQueueRequest = new DeleteQueueRequest(QueueUrl);
            _sqsClient.DeleteQueueAsync(deleteQueueRequest);
            Console.WriteLine("Queue deleted.");
        }
    }
}