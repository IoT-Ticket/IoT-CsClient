using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Wapice.IoTTicket.RestClient.Extensions;
using Wapice.IoTTicket.RestClient.Model;

namespace Wapice.IoTTicket.RestClient.Exceptions
{
    public class IoTServerCommunicationException : IoTTicketException
    {
        public IoTServerCommunicationException(HttpResponseMessage httpResponse)
            : base(httpResponse.ReasonPhrase)
        {
            SetHttpResponseProperties(httpResponse);
        }

        public IoTServerCommunicationException(ErrorInfo errorInfo, HttpResponseMessage httpResponse)
            : base(errorInfo.Description)
        {
            ErrorInfo = errorInfo;
            SetHttpResponseProperties(httpResponse);
        }

        public IoTServerCommunicationException(HttpRequestException innerException) 
            : base("Couldn't communicate with server. See inner exception for more details.", innerException)
        {
            
        }

        private void SetHttpResponseProperties(HttpResponseMessage httpResponse)
        {
            HttpStatusCode = httpResponse.StatusCode;
            ResponseString = httpResponse.GetResponseContentAsString();
        }

        public ErrorInfo ErrorInfo { get; protected set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string ResponseString { get; set; }
    }
}
