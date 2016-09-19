using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Compilers
{
    public class TypeReference : FileReference
    {
        public TypeReference(Type type) { this.Type = type; this.Filename = type.Assembly.Location;}
        public Type Type { get; private set; }
        
        
    }
}
