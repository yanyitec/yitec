using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Compiling
{
    public class ProjectReference : IReference
    {
        public ProjectReference(IProject proj) {
            this.Project = proj;
        }

        public IProject Project { get; private set; }

        


        public string Name
        {
            get { return Project.Name; }
        }





        public void UseStream(Action<Stream> handler)
        {
            using (Stream stream = new MemoryStream()) {
                this.Project.Compile(stream);
            }
        }

        public bool Equals(IReference obj)
        {
            if (obj == null) return false;
            if (obj == this) return true;
            var fr = obj as ProjectReference;
            if (fr != null) return fr.Name == this.Name;
            return false;
        }
    }
}
