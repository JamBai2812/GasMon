using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace GasMon
{
    public class MessageProcessor
    {
        public List<ReadingFromSensor> Readings { get; }
        private int _waitTime = 5;

        public MessageProcessor()
        {
            Readings = new List<ReadingFromSensor>();
        }


        public ReceiveMessageResponse CollectMessages(SubscribedQueue queue, IAmazonSQS sqsClient)
        {
            //Collect Messages
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queue.QueueUrl,
                WaitTimeSeconds = _waitTime
            };
            return sqsClient.ReceiveMessageAsync(receiveMessageRequest).Result;
        }

        public void ProcessMessagesFromResponse(ReceiveMessageResponse messageResponse, List<string> locationIds)
        {
            if (messageResponse.Messages.Count != 0)
            {
                foreach (var message in messageResponse.Messages)
                {
                    ProcessMessage(message, locationIds);
                }
            }
            else
            {
                Console.WriteLine($"No messages found in the last {_waitTime} seconds.");
            }
        }
        

        private void ProcessMessage(Message message, List<string> locations)
        {
            var messageContent = JsonConvert.DeserializeObject<MessageBody>(message.Body);
            var reading = JsonConvert.DeserializeObject<ReadingFromSensor>(messageContent.Message);

            if (locations.Contains(reading.LocationId) && !Readings.Contains(reading))
            {
                Readings.Add(reading);
            }
            else
            {
                Console.WriteLine("Reading from an unchecked sensor.");
            }


            if (Readings.Count != 0)
            {
                var first = Readings.FirstOrDefault(r => r.Timestamp < reading.Timestamp - 5000);
                if (first != null)
                {
                    Readings.Remove(first);
                }
            }
        }
    }
}