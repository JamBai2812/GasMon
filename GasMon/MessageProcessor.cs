using System;
using Amazon.SQS.Model;

namespace GasMon
{
    public class MessageProcessor
    {
        public MessageProcessor()
        {
            
        }

        public void ProcessMessage(Message message)
        {
            Console.WriteLine("     "+message.MessageId);
        }
    }
}