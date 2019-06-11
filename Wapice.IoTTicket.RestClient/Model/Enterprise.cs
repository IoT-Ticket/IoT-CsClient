using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient.Model
{

    [DataContract]
    public class Enterprise
    {
        [DataMember(Name="href")]
        public Uri Uri { get; set; }

        [DataMember(Name="name")]
        public string Name { get; set; }

        [DataMember(Name="resourceId")]
        public string ResourceId { get; set; }

        [DataMember(Name="hasSubEnterprises")]
        public bool HasSubEnterprises { get; set; }
    }
}
