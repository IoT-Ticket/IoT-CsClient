using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient.Model
{

    [System.Runtime.Serialization.DataContract]
    public class Enterprise
    {
        [DataMember(Name="href")]
        public Uri uri { get; set; }

        public string Name { get; set; }
        public string ResourceId { get; set; }
        public bool HasSubEnterprises { get; set; }
    }
}
