using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Compiling
{
    public interface IReference : IEquatable<IReference>
    {
        string Name { get; }
        void UseStream(Action<Stream> handler);
    }
}
