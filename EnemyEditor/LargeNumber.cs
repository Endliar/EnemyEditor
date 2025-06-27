using System;
using System.Numerics;
using System.Globalization;

namespace EnemyEditor
{
    public class LargeNumber
    {
        private BigInteger _value;

        public LargeNumber(long value)
        {
            _value = new BigInteger(value);
        }

        public LargeNumber(BigInteger value)
        {
            _value = value;
        }

        public static LargeNumber operator +(LargeNumber a, LargeNumber b) => new LargeNumber(a._value + b._value);
        public static LargeNumber operator -(LargeNumber a, LargeNumber b) => new LargeNumber(a._value - b._value);
        public static LargeNumber operator *(LargeNumber a, LargeNumber b) => new LargeNumber(a._value * b._value);
        public static LargeNumber operator /(LargeNumber a, LargeNumber b) => new LargeNumber(a._value / b._value);

        public static bool operator >(LargeNumber a, LargeNumber b) => a._value > b._value;
        public static bool operator <(LargeNumber a, LargeNumber b) => a._value < b._value;
        public static bool operator >=(LargeNumber a, LargeNumber b) => a._value >= b._value;
        public static bool operator <=(LargeNumber a, LargeNumber b) => a._value <= b._value;

        public static explicit operator BigInteger(LargeNumber num) => num._value;
        public static explicit operator LargeNumber(long val) => new LargeNumber(val);

        public override string ToString()
        {
            if (_value < 1000)
            {
                return _value.ToString();
            }

            if (_value < 1_000_000) // K
            {
                return $"{((double)_value / 1000.0):0.##}K";
            }
            if (_value < 1_000_000_000) // M
            {
                return $"{((double)_value / 1_000_000.0):0.##}M";
            }
            if (_value < 1_000_000_000_000) // B
            {
                return $"{((double)_value / 1_000_000_000.0):0.##}B";
            }

            return $"{((double)_value / 1_000_000_000_000.0):0.##}T";
        }

        public string ToFullString() => _value.ToString("N0", new CultureInfo("en-US"));
    }
}