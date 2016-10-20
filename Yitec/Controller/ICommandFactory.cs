using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller
{
    public interface ICommandFactory
    {
        ICommand GetOrCreateCommand(RouteData routeData,HttpMethods method,Context context);

        
    }
}
