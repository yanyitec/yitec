using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Yitec.Compilers
{
    public class Project : Yitec.Compilers.IProject
    {
        public Project(string name=null) {
            this._References = new List<IReference>();
            this._Codes = new List<ISource>();
            this.Name = name;
        }
        /// <summary>
        /// 项目名
        /// </summary>
        public string Name { get; set; }

        List<ISource> _Codes;

        public IReadOnlyList<ISource> Codes { get { return _Codes.AsReadOnly(); } }

        public IProject AddSource(ISource source)
        {
            if (_Codes.Exists(p => p.Equals(source))) return this;
            _Codes.Add(source);
            return this;
        }

        public IProject AddFileSource(string filename)
        {
            return this.AddSource(new FileSource(filename));
        }

        public IProject AddTextSource(string text)
        {
            return this.AddSource(new TextSource(text));
        }
        

        List<IReference> _References;

        public IReadOnlyList<IReference> References { 
            get
            {
                return _References.AsReadOnly();
            }
        }

        /// <summary>
        /// 添加引用
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public IProject WithReference(IReference reference)
        {
            if (reference == null) return this;
            var existed = _References.Find(p=>p.Equals(reference));
            if (existed != null) return this;
            _References.Add(reference);
            return this;
        }

        public IProject WithReference(Type type)
        {
            return this.WithReference(new TypeReference(type));
        }

        public void Compile(Stream stream)
        {
            List<SyntaxTree> trees = new List<SyntaxTree>();
            foreach (var code in this._Codes) {
                var content = code.GetContent();
                var tree = SyntaxFactory.ParseSyntaxTree(content);
                trees.Add(tree);
            }

            List<MetadataReference> references = new List<MetadataReference>();

            foreach(var reference in this._References){
                MetadataReference meta = null;
                reference.UseStream((refStream) => {
                    meta = MetadataReference.CreateFromStream(refStream);
                });
                
                references.Add(meta);
            }


            var compilation = CSharpCompilation.Create(
                this.Name,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                syntaxTrees:trees,
                references: references);

            if (compilation.GetDiagnostics().Count()>0) {
                throw new Exception();    
            }

            var compileResult = compilation.Emit(stream);
            if (!compileResult.Success)
            {
                throw new Exception();
            }
        }

        public Assembly CompileToAssembly() {
            using (var stream = new MemoryStream()) {
                this.Compile(stream);
                var asm = Assembly.Load(stream.GetBuffer());
                return asm;
            }
            
            
        }
    }
}
