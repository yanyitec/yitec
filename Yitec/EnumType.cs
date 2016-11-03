using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec
{
    public class EnumType
    {
        readonly static System.Text.RegularExpressions.Regex NumberRegx = new System.Text.RegularExpressions.Regex("^ *\\d+ *$");
        public static bool TryParse<T>(string input, out T value)
            where T:struct
        {
            value = default(T);
            var type = typeof(T);
            if (!type.IsEnum) throw new ArgumentException("<T> is not enum type.");
            if (NumberRegx.IsMatch(input))
            {
                var val = int.Parse(input.Trim());
                value = (T)Enum.ToObject(type, val);
                return true;
            }
            else {
                return Enum.TryParse<T>(input,true,out value);
            }
        }

        public static object ParseEnum(Type type, string input) {
            var val = Enum.Parse(type, input);
            if (val != null) return val;
            int enumValue = 0;
            if (int.TryParse(input, out enumValue)) {
                return Enum.ToObject(type, enumValue);
            }
            return null;
        }
    }
}
