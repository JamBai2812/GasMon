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
        private readonly List<string> _locationIds;
        private const int _waitTime = 5;
        private const long _expiryTime = 5000; //milliseconds


        public MessageProcessor(List<string> locationIds)
        {
            Readings = new List<ReadingFromSensor>();
            _locationIds = locationIds;
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

        public void ProcessMessagesFromResponse(ReceiveMessageResponse messageResponse)
        {
            if (messageResponse.Messages.Count != 0)
            {
                foreach (var message in messageResponse.Messages)
                {
                    ProcessMessage(message);
                }
            }
            else
            {
                Console.WriteLine($"No messages found in the last {_waitTime} seconds.");
            }
        }
        
        private void ProcessMessage(Message message)
        {
            var reading = DeserializeMessage(message);
            FilterAndAddReading(reading);
            RemoveOneExpiredReading(reading);
        }

        private ReadingFromSensor DeserializeMessage(Message message)
        {
            var messageContent = JsonConvert.DeserializeObject<MessageBody>(message.Body);
            return JsonConvert.DeserializeObject<ReadingFromSensor>(messageContent.Message);
        }

        private void FilterAndAddReading(ReadingFromSensor reading)
        {
            if (ReadingFromCheckedLocation(reading) && IsNotDuplicate(reading))
            {
                Readings.Add(reading);
                Console.WriteLine("Collected reading.");
            }
            else
            {
                Console.WriteLine("Found reading from an unchecked sensor.");
            }
        }

        private void RemoveOneExpiredReading(ReadingFromSensor reading)
        {
            if (Readings.Count == 0) return;
            var first = Readings.FirstOrDefault(r => r.Timestamp < reading.Timestamp - _expiryTime);
            if (first != null)
            {
                Readings.Remove(first);
            }
        }
        
        private bool ReadingFromCheckedLocation(ReadingFromSensor reading)
        {
            return _locationIds.Contains(reading.LocationId);
        }

        private bool IsNotDuplicate(ReadingFromSensor reading)
        {
            return !Readings.Contains(reading);
        }
    }
}