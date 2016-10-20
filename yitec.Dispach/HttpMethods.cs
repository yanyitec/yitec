using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    public enum HttpMethods
    {
        
        GET = 1,
        POST = 1 << 1,
        PUT = 1<<2,
        DELETE = 1<<3,
        All = GET | POST | PUT | DELETE
    }
}
