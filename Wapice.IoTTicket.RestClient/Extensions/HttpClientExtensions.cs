using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Wapice.IoTTicket.RestClient.Exceptions;

namespace Wapice.IoTTicket.RestClient.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> HandledGetAsync(this HttpClient client, string requestUri, CancellationToken cancellationToken)
        {
            try
            {
                var response = await client.GetAsync(requestUri, cancellationToken);
                await response.ThrowIfUnsuccessfulAsync();

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new IoTServerCommunicationException(ex);
            }
        }

        public static async Task<HttpResponseMessage> HandledPostAsync(this HttpClient client, string requestUri, HttpContent content, CancellationToken cancellationToken)
        {
            try
            {
                var response = await client.PostAsync(requestUri, content, cancellationToken);
                await response.ThrowIfUnsuccessfulAsync();

                return response;
            }
            catch (HttpRequestException ex)
            {
                throw new IoTServerCommunicationException(ex);
            }
        }
    }
}
