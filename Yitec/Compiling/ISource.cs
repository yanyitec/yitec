using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Compiling
{
    public interface ISource :IEquatable<ISource>
    {
        string GetContent();
    }
}
