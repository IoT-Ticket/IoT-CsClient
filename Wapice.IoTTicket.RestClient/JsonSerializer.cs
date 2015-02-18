using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient
{
    public static class JsonSerializer
    {
        public static string Serialize<T>(T data) where T : class
        {
            var memStream = new MemoryStream();
            var jsonSerializer = GetJsonSerializer<T>();
            jsonSerializer.WriteObject(memStream, data);
            memStream.Seek(0, SeekOrigin.Begin);
            
            var reader = new StreamReader(memStream);
            return reader.ReadToEnd();
        }

        public static async Task<T> DeserializeAsync<T>(HttpContent content) where T : class
        {
            var jsonSerializer = GetJsonSerializer<T>();
            using (var stream = await content.ReadAsStreamAsync())
            {
                return jsonSerializer.ReadObject(stream) as T;
            }
        }

        private static DataContractJsonSerializer GetJsonSerializer<T>() where T : class
        {
            var jsonSerializer = new DataContractJsonSerializer(typeof (T), new DataContractJsonSerializerSettings
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-dd'T'HH:mm:ss'UTC'")
            });

            return jsonSerializer;
        }
    }
}
