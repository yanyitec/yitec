using System;
using System.IO;
using System.Reflection;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace APISampleUnitTestsCS
{
    public static class Compilations
    {
        public static void Main()
        {
            var expression = "6 * 7";
            var text = @"public class Calculator 
{ 
    public static object Evaluate() 
    { 
        return $; 
    }  
}".Replace("$", expression);

            var proj = new Yitec.Compiling.Project("calc1.dll");
            proj.WithReference(typeof(object));
            proj.AddTextSource(text);
            var compiledAssembly1 = proj.CompileToAssembly();

            var tree = SyntaxFactory.ParseSyntaxTree(text);
            var compilation = CSharpCompilation.Create(
                "calc.dll",
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
                syntaxTrees: new[] { tree },
                references: new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });


            Assembly compiledAssembly;
            using (var stream = new MemoryStream())
            {
                var compileResult = compilation.Emit(stream);
                compiledAssembly = Assembly.Load(stream.GetBuffer());
            }


            Type calculator = compiledAssembly1.GetType("Calculator");
            MethodInfo evaluate = calculator.GetMethod("Evaluate");
            string answer = evaluate.Invoke(null, null).ToString();


        }


//        public void GetErrorsAndWarnings()
//        {
//            string text = @"class Program 
//{ 
//    static int Main(string[] args) 
//    { 
//    } 
//}";


//            var tree = SyntaxFactory.ParseSyntaxTree(text);
//            var compilation = CSharpCompilation
//                .Create("program.exe")
//                .AddSyntaxTrees(tree)
//                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));


//            IEnumerable<Diagnostic> errorsAndWarnings = compilation.GetDiagnostics();
            


//            Diagnostic error = errorsAndWarnings.First();
//            Assert.Equal(
//                "'Program.Main(string[])': not all code paths return a value",
//                error.GetMessage(CultureInfo.InvariantCulture));


//            Location errorLocation = error.Location;
//            Assert.Equal(4, error.Location.SourceSpan.Length);


//            SourceText programText = errorLocation.SourceTree.GetText();
//            Assert.Equal("Main", programText.ToString(errorLocation.SourceSpan));


//            FileLinePositionSpan span = error.Location.GetLineSpan();
//            Assert.Equal(15, span.StartLinePosition.Character);
//            Assert.Equal(2, span.StartLinePosition.Line);
//        }
    }
}
