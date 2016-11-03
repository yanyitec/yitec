using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec
{
    public static class ValueTypes
    {
        static IDictionary<int, Func<string, object>> parsers = new SortedDictionary<int, Func<string, object>>() {
             { typeof(byte).GetHashCode(),new Func<string,object>(ParseByte)}
            ,{ typeof(bool).GetHashCode(),new Func<string,object>(ParseBoolean)}
            ,{ typeof(short).GetHashCode(),new Func<string,object>(ParseInt16)}
            ,{ typeof(ushort).GetHashCode(),new Func<string,object>(ParseUInt16)}
            ,{ typeof(int).GetHashCode(),new Func<string,object>(ParseInt32)}
            ,{ typeof(uint).GetHashCode(),new Func<string,object>(ParseUInt32)}
            ,{ typeof(long).GetHashCode(),new Func<string,object>(ParseInt64)}
            ,{ typeof(ulong).GetHashCode(),new Func<string,object>(ParseUInt64)}
            ,{ typeof(decimal).GetHashCode(),new Func<string,object>(ParseDecimal)}
            ,{ typeof(float).GetHashCode(),new Func<string,object>(ParseSingle)}
            ,{ typeof(double).GetHashCode(),new Func<string,object>(ParseDouble)}
            ,{ typeof(Guid).GetHashCode(),new Func<string,object>(ParseGuid)}
            ,{ typeof(DateTime).GetHashCode(),new Func<string,object>(ParseDateTime)}
        };
        public static Func<string, object> GetParser(Type type) {
            Func<string, object> result = null;
            if (parsers.TryGetValue(type.GetHashCode(), out result)) return result;
            if (type.IsEnum) {
                return (input) => {
                    int val = 0;
                    if (int.TryParse(input, out val))
                    {
                        return Enum.ToObject(type, val);
                    }
                    else {
                        var obj = Enum.Parse(type, input);
                        if (obj != null) return obj;
                    }                    
                    return null;
                };
            }
            return result;
        }

        public static object GetDefaultValue(Type type)
        {
            if (type == typeof(string)) return null;
            if (
                type == typeof(byte)
                || type == typeof(short)
                || type == typeof(ushort)
                || type == typeof(int)
                || type == typeof(uint)
                || type == typeof(long)
                || type == typeof(ulong)
                || type == typeof(decimal)
                || type == typeof(float)
                || type == typeof(double)
                )
            {
                return 0;
            }
            if (type == typeof(Guid))
            {
                return Guid.Empty;
            }
            if (type == typeof(DateTime))
            {
                return DateTime.MinValue;
            }
            if (type.IsEnum)
            {
                return 0;
            }
            if (type.GUID == NullableTypes.GeneralNullableType.GUID) return NullableTypes.GetDefaultValue(new NullableTypes(type));
            return null;
        }
        public static object ParseByte(string input)
        {
            System.Byte value = 0;
            if (byte.TryParse(input, out value)) return value;
            return null;
        }

        public static object ParseBoolean(string input)
        {
            if (input == null || input == string.Empty) return null;
            input = input.Trim();
            int val = 0;
            if (int.TryParse(input, out val)) return val != 0;
            input = input.ToLower();
            if (input == "true") return true;
            if (input == "false") return false;
            if (input == "on") return true;
            if (input == "off") return false;
            return null;
        }

        public static object ParseInt16(string input)
        {
            short value = 0;
            if (short.TryParse(input, out value)) return value;
            return null;
        }

        public static object ParseUInt16(string input)
        {
            System.UInt16 value = 0;
            if (ushort.TryParse(input, out value)) return value;
            return null;
        }

        public static object ParseInt32(string input)
        {
            System.Int32 value = 0;
            if (int.TryParse(input, out value)) return value;
            return null;
        }

        public static object ParseUInt32(string input)
        {
            System.UInt32 value = 0;
            if (uint.TryParse(input, out value)) return value;
            return null;
        }

        public static object ParseInt64(string input)
        {
            System.Int64 value = 0;
            if (long.TryParse(input, out value)) return value;
            return null;
        }

        public static object ParseUInt64(string input)
        {
            System.UInt64 value = 0;
            if (ulong.TryParse(input, out value)) return value;
            return null;
        }

        public static object ParseDecimal(string input)
        {
            System.Decimal value = 0;
            if (decimal.TryParse(input, out value)) return value;
            return null;
        }

        public static object ParseDouble(string input)
        {
            System.Double value = 0;
            if (double.TryParse(input, out value)) return value;
            return null;
        }

        public static object ParseSingle(string input)
        {
            System.Single value = 0;
            if (float.TryParse(input, out value)) return value;
            return null;
        }

        public static object ParseGuid(string input)
        {
            System.Guid value = Guid.Empty;
            if (Guid.TryParse(input, out value)) return value;
            return null;
        }

        public static object ParseDateTime(string input)
        {
            System.DateTime value = System.DateTime.MinValue;
            if (DateTime.TryParse(input, out value)) return value;
            return null;
        }
        //public static object ParseEnum(Type type, string input) { }

        public static T ParseEnum<T>(string input)
            where T : struct
        {
            T value = default(T);
            if (EnumType.TryParse<T>(input, out value)) return value;
            return default(T);
        }
    }
}
