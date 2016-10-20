using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec
{
    public class NullableType
    {
        static Type GenericNullableType = typeof(Nullable<>);
        public NullableType(Type type)
        {
            if (type.GUID == GenericNullableType.GUID)
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

        public static T? ParseEnum<T>(string input)
            where T : struct
        {
            T value = default(T);
            if (EnumType.TryParse<T>(input, out value)) return new Nullable<T>(value);
            return null;
        }

    }
}
