using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wapice.IoTTicket.RestClient.Exceptions;
using Wapice.IoTTicket.RestClient.Model;

namespace Wapice.IoTTicket.RestClient.Extensions
{
    public static class HttpResponseMessageExtensions
    {
        public static async Task ThrowIfUnsuccessfulAsync(this HttpResponseMessage httpResponse)
        {
            if (httpResponse.IsSuccessStatusCode)
                return;

            try
            {
                var errorInfo = await JsonSerializer.DeserializeAsync<ErrorInfo>(httpResponse.Content);
                throw new IoTServerCommunicationException(errorInfo, httpResponse);
            }
            catch (SerializationException)
            {
                var exec = new IoTServerCommunicationException(httpResponse);
                throw exec;
            }
        }

        public static string GetResponseContentAsString(this HttpResponseMessage httpResponse)
        {
            if (httpResponse.Content != null)
                return httpResponse.Content.ReadAsStringAsync().Result;

            return String.Empty;
        }
    }
}
