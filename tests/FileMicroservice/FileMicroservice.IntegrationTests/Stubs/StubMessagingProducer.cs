using FileMicroservice.DTOs;
using FileMicroservice.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMicroservice.IntegrationTests.Stubs
{
    public class StubMessagingProducer : IMessagingProducer
    {
        public void SendMessage<T>(T message, string routingKey)
        {
            //throw new NotImplementedException();
        }
    }
}
