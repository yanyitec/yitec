using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace yitec.Dispach
{
    public interface ICommandLoader
    {
        ICommandFactory Load();
    }
}
