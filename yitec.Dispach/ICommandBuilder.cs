using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    public interface ICommandBuilder
    {
        Type BuildCommand(MethodInfo methodInfo,IBinderFactory binderFactory);
    }
}
