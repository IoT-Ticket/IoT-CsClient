using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient.Model
{
    [DataContract]
    [KnownType("GetTypes")]
    public class PagedResult<T>
    {
        [DataMember(Name = "items")]
        public IEnumerable<T> Result { get; set; }

        [DataMember(Name = "offset")]
        public int Skip { get; set; }
        [DataMember(Name = "limit")]
        public int RequestedCount { get; set; }
        [DataMember(Name = "fullSize")]
        public int TotalCount { get; set; }

        public bool ContainsAnyItem { get { return Result != null && Result.Any(); }}

        private static Type[] GetTypes()
        {
            return new[] {typeof (T)};
        }
    }
}
