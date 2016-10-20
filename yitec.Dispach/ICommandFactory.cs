using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yitec.Dispach
{
    public interface ICommandFactory
    {
        ICommand GetOrCreateCommand(RouteData routeData,HttpMethods method,Context context);

        
    }
}
