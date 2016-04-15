using System;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public sealed class DynamicID<TEnum> : IConvertible where TEnum : struct
    {
        private DynamicID(int value)
        {
            Value = value;
        }

        private int Value { get; }

        public static implicit operator DynamicID<TEnum>(TEnum e)
        {
            return new DynamicID<TEnum>(Convert.ToInt32(e));
        }

        public static implicit operator DynamicID<TEnum>(int i)
        {
            return new DynamicID<TEnum>(i);
        }

        public static implicit operator int(DynamicID<TEnum> e)
        {
            return e.Value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        private bool Equals(DynamicID<TEnum> other)
        {
            return Value == other.Value;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DynamicID<TEnum> && Equals((DynamicID<TEnum>)obj);
        }

        public override int GetHashCode()
        {
            return Value;
        }

        TypeCode IConvertible.GetTypeCode() => TypeCode.Object;
        bool IConvertible.ToBoolean(IFormatProvider provider) => Convert.ToBoolean(Value);
        char IConvertible.ToChar(IFormatProvider provider) => Convert.ToChar(Value);
        sbyte IConvertible.ToSByte(IFormatProvider provider) =>  Convert.ToSByte(Value);
        byte IConvertible.ToByte(IFormatProvider provider) => Convert.ToByte(Value);
        short IConvertible.ToInt16(IFormatProvider provider) => Convert.ToInt16(Value);
        ushort IConvertible.ToUInt16(IFormatProvider provider) => Convert.ToUInt16(Value);
        int IConvertible.ToInt32(IFormatProvider provider) => Value;
        uint IConvertible.ToUInt32(IFormatProvider provider) => Convert.ToUInt32(Value);
        long IConvertible.ToInt64(IFormatProvider provider) => Convert.ToInt64(Value);
        ulong IConvertible.ToUInt64(IFormatProvider provider) => Convert.ToUInt64(Value);
        float IConvertible.ToSingle(IFormatProvider provider) => Convert.ToSingle(Value);
        double IConvertible.ToDouble(IFormatProvider provider) => Convert.ToDouble(Value);
        decimal IConvertible.ToDecimal(IFormatProvider provider) => Convert.ToDecimal(Value);
        DateTime IConvertible.ToDateTime(IFormatProvider provider) => Convert.ToDateTime(Value);
        string IConvertible.ToString(IFormatProvider provider) => Convert.ToString(Value);
        object IConvertible.ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)Value).ToType(conversionType, provider);
    }
}
