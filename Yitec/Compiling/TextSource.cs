using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Compiling
{
    public class TextSource: ISource
    {
        public TextSource(string content) { this.Content = content; }

        public string Content { get; private set; }
        public string GetContent()
        {
            return this.Content;
        }

        public bool Equals(ISource other)
        {
            if (other == null) return false;
            if (other == this) return true;
            var ot = other as TextSource;
            if (ot != null) { return ot.Content == this.Content; }
            return false;
        }
    }
}
