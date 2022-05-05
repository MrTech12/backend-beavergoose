using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileMicroservice.Interfaces;

namespace FileMicroservice.UnitTests.Stubs
{
    public class StubMessagingProducer : IMessagingProducer
    {
        public void SendMessage<T>(T message, string routingKey)
        {
            //throw new NotImplementedException();
        }
    }
}
