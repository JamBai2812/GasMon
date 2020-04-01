using System;
using System.Collections.Generic;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace GasMon
{
    public class MessageProcessor
    {
        public MessageProcessor()
        {
            
        }

        public void ProcessMessage(Message message, List<Location> locations)
        {
            var messageContent = JsonConvert.DeserializeObject<MessageBody>(message.Body);
            var sensor = JsonConvert.DeserializeObject<SensorFromMessage>(messageContent.Message);
            Console.WriteLine("Sensor:");
            Console.WriteLine("Location Id: "+sensor.LocationId);
            Console.WriteLine("Value: "+sensor.Value);
            Console.WriteLine("Timestamp: "+sensor.Timestamp);
            Console.WriteLine();

            // Console.WriteLine(messageContent.MessageId);
            // Console.WriteLine(body);

        }
    }
}