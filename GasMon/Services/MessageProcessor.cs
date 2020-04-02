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

        public MessageProcessor()
        {
            Readings = new List<ReadingFromSensor>();
        }


        public ReceiveMessageResponse CollectMessages(SubscribedQueue queue, int waitTime, IAmazonSQS sqsClient)
        {
            //Collect Messages
            var receiveMessageRequest = new ReceiveMessageRequest
            {
                QueueUrl = queue.QueueUrl,
                WaitTimeSeconds = waitTime
            };
            return sqsClient.ReceiveMessageAsync(receiveMessageRequest).Result;
        }

        public void ProcessMessage(Message message, List<string> locations)
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