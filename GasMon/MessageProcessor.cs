using System;
using Amazon.SQS.Model;
using Newtonsoft.Json;

namespace GasMon
{
    public class MessageProcessor
    {
        public MessageProcessor()
        {
            
        }

        public void ProcessMessage(Message message)
        {
            var body = message.Body;

            var messageContent = JsonConvert.DeserializeObject<MessageBody>(body);
            var sensor = JsonConvert.DeserializeObject<SensorFromMessage>(messageContent.Message);
            
            Console.WriteLine(sensor.LocationId);

            // Console.WriteLine(messageContent.MessageId);
            // Console.WriteLine(body);

        }
    }
}