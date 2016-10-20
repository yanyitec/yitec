using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller
{
    public interface IParameterBinderTypeFactory
    {
        Type GetBinderType(ParameterInfo paramInfo , string commandText,HttpMethods method );
    }
}
