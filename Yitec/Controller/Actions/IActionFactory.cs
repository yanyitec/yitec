using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller.Actions
{
    public interface IActionFactory
    {
        IAction GetOrCreateCommand(RouteData routeData,HttpMethods method,Context context);

        
    }
}
