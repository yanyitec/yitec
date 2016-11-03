using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yitec.Controller.Actions
{
    public interface ICommandLoader
    {
        IActionFactory Load();
    }
}
