using System;
using System.Net;
using System.Runtime.Serialization;
using Wapice.IoTTicket.RestClient.Model;

namespace Wapice.IoTTicket.RestClient.Exceptions
{
    public class IoTTicketException : Exception
    {
        public IoTTicketException(string message) : base(message)
        {
        }

        public IoTTicketException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
