using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Compilers
{
    public class FileReference : IReference
    {
        public FileReference(string filename) {
            this.Filename = filename;
            this.Name = filename;
        }

        protected FileReference() { }


        public string Name { get;private set; }

        public string Filename { get; protected set; }

        public void UseStream(Action<Stream> handler) {
            using (var stream = System.IO.File.OpenRead(this.Filename)) {
                handler(stream);
            }
        }

        public bool Equals(IReference obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            var fr = obj as FileReference;
            if (fr != null) return fr.Filename == this.Filename;
            return false;
        }
    }
}
