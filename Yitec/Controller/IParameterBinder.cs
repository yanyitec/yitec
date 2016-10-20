using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller
{
    public interface IParameterBinder
    {
        
        bool TryGetValue(string name,Type type,IArguments arguments,IRequest request, Context context,out object result);
    }
}
