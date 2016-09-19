using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Compilers
{
    public interface IReference : IEquatable<IReference>
    {
        string Name { get; }
        void UseStream(Action<Stream> handler);
    }
}
