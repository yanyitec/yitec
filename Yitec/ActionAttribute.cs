using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec
{
    public class ActionAttribute : Attribute
    {
        public ActionAttribute(string cmdText) {
            this.CommandText = cmdText;
        }

        public string CommandText { get; private set; }
    }
}
