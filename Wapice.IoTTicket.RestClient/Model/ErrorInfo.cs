using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Wapice.IoTTicket.RestClient.Model
{
    [DataContract]
    public class ErrorInfo
    {
        public enum ErrorCodeEnum
        {
            Unknown = 0,
            InternalServerError = 8000,
            PermissionNotSufficient = 8001,
            QuotaViolation = 8002,
            BadInputParamter = 8003,
            WriteFailed = 8004
        }

        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "code")]
        public int Code { get; set; }

        public ErrorCodeEnum CodeEnum
        {
            get { return (ErrorCodeEnum) Code; }
        }

        [DataMember(Name = "moreInfo")]
        public Uri MoreInfoUrl { get; set; }
        [DataMember(Name = "apiver")]
        public int ApiVersion { get; set; }
    }
}
