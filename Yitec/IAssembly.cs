using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;



namespace Yitec
{
    public interface IAssembly
    {
        IEnumerable<Type> GetTypes();

        byte[] GetResource(string path);
    }
}
