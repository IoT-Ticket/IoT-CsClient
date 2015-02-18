using System;
using Wapice.IoTTicket.RestClient.Exceptions;

namespace Wapice.IoTTicket.RestClient.Model
{
    public static class DataTypeFactory
    {
        public static dynamic GetDataType(dynamic value)
        {
            if (value is double || value is float)
                return new DoubleDataType((double) value);

            if (value is Int64 || value is Int32 || value is Int16 || value is Byte)
                return new LongDataType(Convert.ToInt64(value));

            if (value is string)
                return new StringDataType((string) value);

            if (value is bool)
                return new BooleanDataType((bool) value);
            
            if (value is byte[])
                return new BinaryDataType((byte[])value);

            throw new IoTTicketException(String.Format("Unsupported data type: {0}", value.GetType().ToString()));
        }

        public static dynamic GetDataType(string value, string type)
        {
            switch (type)
            {
                case DoubleDataType.TypeName :
                    return new DoubleDataType(Convert.ToDouble(value));
                case LongDataType.TypeName:
                    return new LongDataType(Convert.ToInt64(value));
                case StringDataType.TypeName:
                    return new StringDataType(value);
                case BooleanDataType.TypeName:
                    return new BooleanDataType(Convert.ToBoolean(value));
                case BinaryDataType.TypeName:
                    return new BinaryDataType(Convert.FromBase64String(value));
                default:
                    throw new IoTTicketException(String.Format("Unsupported data type: {0}", type));
            }
        }
    }

    public abstract class DataTypeBase<T>
    {
        public T Value { get; set; }
        public virtual dynamic SerializedValue { get { return Value; } }
        public abstract string Name { get; }
    }

    public sealed class DoubleDataType : DataTypeBase<double>
    {
        public const string TypeName = "double";

        public DoubleDataType(double value)
        {
            Value = value;
        }

        public override string Name { get { return TypeName; }}
    }

    public sealed class LongDataType : DataTypeBase<long>
    {
        public const string TypeName = "long";

        public LongDataType(long value)
        {
            Value = value;
        }

        public override string Name { get { return TypeName; } }
    }

    public sealed class StringDataType : DataTypeBase<string>
    {
        public const string TypeName = "string";

        public StringDataType(string value)
        {
            Value = value;
        }

        public override string Name { get { return TypeName; } }
    }

    public class BooleanDataType : DataTypeBase<bool>
    {
        public const string TypeName = "boolean";

        public BooleanDataType(bool value)
        {
            Value = value;
        }

        public override string Name { get { return TypeName; } }
    }

    public class BinaryDataType : DataTypeBase<byte[]>
    {
        public const string TypeName = "binary";

        public BinaryDataType(byte[] value)
        {
            Value = value;
        }

        public override string Name { get { return TypeName; } }

        public override dynamic SerializedValue
        {
            get { return Convert.ToBase64String(Value); }
        }
    }
}
