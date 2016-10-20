using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    public interface IBinder
    {
        object Bind(IArguments arguments, string parameterName, Type parameterType, IEnumerable<Attribute> parameterAttrs);
    }
}
