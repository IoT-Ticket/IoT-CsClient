using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Wapice.IoTTicket.RestClient.Extensions;

namespace Wapice.IoTTicket.RestClient.Model
{
    [DataContract]
    public abstract class DatanodeBase
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "unit", EmitDefaultValue = false)]
        public string Unit { get; set; }
        [DataMember(Name = "dataType")]
        public virtual string DataType { get; set; }
    }

    [DataContract]
    public class DatanodeDetail : DatanodeBase
    {
        [DataMember(Name = "href")]
        public Uri Url { get; set; }
    }

    [DataContract]
    public class DatanodeReadValue : DatanodeBase
    {
        private IEnumerable<DatanodeValueData> _values;

        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "values")]
        public IEnumerable<DatanodeValueData> Values
        {
            get { return _values; }
            set
            {
                _values = value; 
                _values.ToList().ForEach(v => v.Datanode = this);
            }
        }
    }

    [DataContract]
    public class DatanodeWritableValue : DatanodeBase
    {
        [DataMember(Name = "path", EmitDefaultValue = false)]
        public string Path { get; set; }

        [DataMember(Name = "v")]
        public dynamic SerializedValue
        {
            get
            {
                if (_valueType != null)
                    return _valueType.SerializedValue;
                
                return null;
            }
            private set
            {
                throw new NotSupportedException();
            }
        }

        private dynamic _valueType;
        public dynamic Value
        {
            get { return _valueType.Value; }
            set
            {
                _valueType = DataTypeFactory.GetDataType(value);
            }
        }
        [DataMember(Name = "ts", EmitDefaultValue = false)]
        public long? UnixTimestamp { get; set; }

        [DataMember(Name = "dataType")]
        public override string DataType
        {
            get
            {
                if (_valueType != null)
                    return _valueType.Name;

                return String.Empty;
            }
            set { throw new NotSupportedException(); }
        }

        [IgnoreDataMember]
        public DateTime? Timestamp
        {
            get
            {
                if (UnixTimestamp.HasValue)
                    return UnixTimestamp.Value.FromUnixTime();
                
                return null;
            }
            set
            {
                if (value.HasValue)
                    UnixTimestamp = value.Value.ToUnixTime();
                else
                    UnixTimestamp = null;
            }
        }
    }
}
