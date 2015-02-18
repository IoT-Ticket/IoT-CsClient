using System;
using System.Runtime.Serialization;
using Wapice.IoTTicket.RestClient.Extensions;

namespace Wapice.IoTTicket.RestClient.Model
{
    //[DataContract]
    //public class Datapoint
    //{
    //    [DataMember(Name = "name")]
    //    public string Name { get; set; }
    //    [DataMember(Name = "path")]
    //    public string Path { get; set; }

    //    private dynamic _value;
    //    [DataMember(Name = "v")] 
    //    public dynamic Value
    //    {
    //        get { return _value.Value; }
    //        set
    //        {
    //            _value = DataTypeFactory.GetDataType(value);
    //        }
    //    }
    //    [DataMember(Name = "ts")]
    //    public long UnixTimestamp { get; set; }
    //    [DataMember(Name = "unit")]
    //    public string Unit { get; set; }

    //    [DataMember(Name = "dataType")]
    //    public string DataType
    //    {
    //        get
    //        {
    //            if (_value != null)
    //                return _value.Name;

    //            return String.Empty;
    //        }
    //        private set { throw new NotSupportedException(); }
    //    }

    //    [IgnoreDataMember]
    //    public DateTime Timestamp
    //    {
    //        get { return UnixTimestamp.FromUnixTime(); }
    //        set { UnixTimestamp = value.ToUnixTime(); }
    //    }
    //}
}
