using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Yitec.Controller
{
    public interface ICommandLoader
    {
        ICommandFactory Load();
    }
}
