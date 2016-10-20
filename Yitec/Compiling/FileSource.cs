using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Compiling
{
    public class FileSource : ISource
    {
        public FileSource(string filename) {
            this.Filename = filename;
        }

        public string Filename { get; private set; }
        public string GetContent()
        {
            return System.IO.File.ReadAllText(this.Filename);
        }

        public bool Equals(ISource other)
        {
            if (other == null) return false;
            if (other == this) return true;
            var ot = other as FileSource;
            if (ot != null) { return ot.Filename.ToLower() == this.Filename.ToLower(); }
            return false;
        }
    }
}
