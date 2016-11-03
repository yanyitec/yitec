using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Yitec.Tests
{
    [TestClass]
    public class ProjectTest
    {
        [TestMethod]
        public void CompileToAssembly()
        {
            var proj = new Compiling.Project();
            proj.WithReference(typeof(object));
            proj.WithReference(typeof(Guid));
            var asm = proj.CompileToAssembly();
        }
    }
}
