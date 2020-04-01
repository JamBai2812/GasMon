using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace GasMon
{
    public class MessageProcessor
    {
        public MessageProcessor()
        {
            
        }

        public void ProcessMessage(Message message, List<string> locations)
        {
            var messageContent = JsonConvert.DeserializeObject<MessageBody>(message.Body);
            var sensor = JsonConvert.DeserializeObject<SensorFromMessage>(messageContent.Message);

            if (locations.Contains(sensor.LocationId))
            {
                Console.WriteLine(sensor.LocationId);
            }
            else
            {
                Console.WriteLine("Not a checked sensor.");
            }
        }
    }
}