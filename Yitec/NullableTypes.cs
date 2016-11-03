using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yitec
{
    public class NullableTypes
    {
        public static Type GeneralNullableType = typeof(Nullable<>);
        readonly static Type NullableTypeType = typeof(NullableTypes);
        readonly static Dictionary<int, Func<string, object>> Parsers = new Dictionary<int, Func<string, object>>{
            { typeof(byte).GetHashCode(),(input)=>NullableTypes.ParseByte(input)}
            ,{ typeof(short).GetHashCode(),(input)=>NullableTypes.ParseInt16(input)}
            ,{ typeof(ushort).GetHashCode(),(input)=>NullableTypes.ParseUInt16(input)}
            ,{ typeof(int).GetHashCode(),(input)=>NullableTypes.ParseInt32(input)}
            ,{ typeof(uint).GetHashCode(),(input)=>NullableTypes.ParseUInt32(input)}
            ,{ typeof(long).GetHashCode(),(input)=>NullableTypes.ParseInt64(input)}
            ,{ typeof(ulong).GetHashCode(),(input)=>NullableTypes.ParseUInt64(input)}
            ,{ typeof(float).GetHashCode(),(input)=>NullableTypes.ParseSingle(input)}
            ,{ typeof(double).GetHashCode(),(input)=>NullableTypes.ParseDouble(input)}
            ,{ typeof(bool).GetHashCode(),(input)=>NullableTypes.ParseBoolean(input)}
            ,{ typeof(decimal).GetHashCode(),(input)=>NullableTypes.ParseDecimal(input)}
            ,{ typeof(Guid).GetHashCode(),(input)=>NullableTypes.ParseGuid(input)}
            ,{ typeof(DateTime).GetHashCode(),(input)=>NullableTypes.ParseDateTime(input)}

        };
        public NullableTypes(Type type)
        {
            if (type.GUID == GeneralNullableType.GUID)
            {
                this.IsNullable = true;
                this.OrignalType = type;
                this.ActualType = type.GetGenericArguments()[0];
            }
            else
            {
                this.IsNullable = false;
                this.OrignalType = type;
                this.ActualType = type;
            }
        }

        public static Type MakeGeneric(Type genericType) {
            return GeneralNullableType.MakeGenericType(genericType);
        }

        public static implicit operator NullableTypes(Type rawType) {
            return new NullableTypes(rawType);
        }

        
        /// <summary>
        /// 是否是Nullable
        /// </summary>
        public bool IsNullable { get; private set; }
        /// <summary>
        /// 原始的类型
        /// </summary>
        public Type OrignalType { get; private set; }
        /// <summary>
        /// Nullable里面的类型
        /// </summary>
        public Type ActualType { get; private set; }

        public static object GetDefaultValue(NullableTypes nullableType) {
            var type = nullableType.ActualType;
            if (type == typeof(byte)) return new Nullable<byte>();
            if (type == typeof(short)) return new Nullable<short>();
            if (type == typeof(ushort)) return new Nullable<ushort>();
            if (type == typeof(int)) return new Nullable<int>();
            if (type == typeof(uint)) return new Nullable<uint>();
            if (type == typeof(long)) return new Nullable<long>();
            if (type == typeof(ulong)) return new Nullable<ulong>();
            if (type == typeof(decimal)) return new Nullable<decimal>();
            if (type == typeof(float)) return new Nullable<float>();
            if (type == typeof(double)) return new Nullable<double>();
            if (type == typeof(Guid)) return new Nullable<Guid>();
            if (type == typeof(DateTime)) return new Nullable<DateTime>();
            if (type == typeof(DateTime)) return new Nullable<DateTime>();
            if (type.IsEnum) return GeneralNullableType.MakeGenericType(type).GetConstructor(Type.EmptyTypes).Invoke(new object[] { });
            return null;
        }
        

        public static Func<string, object> GetParser(NullableTypes type)
        {
            if (!type.IsNullable) return null;
            Func<string, object> result = null;
            if (Parsers.TryGetValue(type.ActualType.GetHashCode(), out result)) return result;
            
            return result;
        }


        public static byte? ParseByte(string input)
        {
            System.Byte value = 0;
            if (byte.TryParse(input, out value)) return value;
            return null;
        }

        public static bool? ParseBoolean(string input)
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

        public static short? ParseInt16(string input)
        {
            short value = 0;
            if (short.TryParse(input, out value)) return value;
            return null;
        }

        public static ushort? ParseUInt16(string input)
        {
            System.UInt16 value = 0;
            if (ushort.TryParse(input, out value)) return value;
            return null;
        }

        public static int? ParseInt32(string input)
        {
            System.Int32 value = 0;
            if (int.TryParse(input, out value)) return value;
            return null;
        }

        public static uint? ParseUInt32(string input)
        {
            System.UInt32 value = 0;
            if (uint.TryParse(input, out value)) return value;
            return null;
        }

        public static long? ParseInt64(string input)
        {
            System.Int64 value = 0;
            if (long.TryParse(input, out value)) return value;
            return null;
        }

        public static ulong? ParseUInt64(string input)
        {
            System.UInt64 value = 0;
            if (ulong.TryParse(input, out value)) return value;
            return null;
        }

        public static decimal? ParseDecimal(string input)
        {
            System.Decimal value = 0;
            if (decimal.TryParse(input, out value)) return value;
            return null;
        }

        public static double? ParseDouble(string input)
        {
            System.Double value = 0;
            if (double.TryParse(input, out value)) return value;
            return null;
        }

        public static float? ParseSingle(string input)
        {
            System.Single value = 0;
            if (float.TryParse(input, out value)) return value;
            return null;
        }

        public static Guid? ParseGuid(string input)
        {
            System.Guid value = Guid.Empty;
            if (Guid.TryParse(input, out value)) return value;
            return null;
        }

        public static DateTime? ParseDateTime(string input)
        {
            System.DateTime value = System.DateTime.MinValue;
            if (DateTime.TryParse(input, out value)) return value;
            return null;
        }

        static System.Text.RegularExpressions.Regex NumberRegx = new System.Text.RegularExpressions.Regex("^\\d+$");

        public static T? ParseEnum<T>(string input)
            where T : struct
        {
            T value = default(T);
            if (EnumType.TryParse<T>(input, out value)) return new Nullable<T>(value);
            
            return null;
        }

    }
}
