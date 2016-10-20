using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    public interface IRequest
    {
        HttpMethods Method { get; }
        
        string Text { get; }

        IDictionary<string, string> Parameters { get; }

        string this[string key] { get; }
    }
}
