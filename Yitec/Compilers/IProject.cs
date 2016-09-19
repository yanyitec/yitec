using System;
namespace Yitec.Compilers
{
    public interface IProject
    {
        IProject AddSource(ISource source);

        IProject AddFileSource(string filename);

        IProject AddTextSource(string text);
        System.Collections.Generic.IReadOnlyList<ISource> Codes { get; }
        void Compile(System.IO.Stream stream);
        System.Reflection.Assembly CompileToAssembly();
        string Name { get; set; }
        System.Collections.Generic.IReadOnlyList<IReference> References { get; }
        IProject WithReference(Type type);
        IProject WithReference(IReference reference);
    }
}
