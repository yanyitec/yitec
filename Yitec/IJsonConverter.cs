using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec
{
    public interface IJsonConverter
    {
        string ToJson(object obj);

        T Parse<T>(string jsonText);
    }
}
