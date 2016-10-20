using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Controller
{
    public interface IControllerFactoryFactory
    {
        Func<ICommand , RouteData , Context ,object> GetOrCreateControllerFactory(Type controllerType);
    }
}
