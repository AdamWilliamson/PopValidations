using PopValidations;
using System.Collections.Generic;
using Xunit;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis.Emit;
using System;
using ApprovalTests;

namespace PopValidations_Tests.PublicAccessTests;

public record TestClass(
    int IntField,
    int? NullableIntField,
    List<int> ListOfInts,
    List<int?> ListOfNullableInts,
    List<int>? NullableListOfInts,
    List<int?>? NullableListOfNullableInts
);

public class TestClassValidator : AbstractValidator<TestClass>
{
    public TestClassValidator()
    {
        Describe(x => x.IntField);
        Describe(x => x.NullableIntField);
        Describe(x => x.ListOfInts);
        Describe(x => x.ListOfNullableInts);
        Describe(x => x.NullableListOfInts);
        Describe(x => x.NullableListOfNullableInts);

        DescribeEnumerable(x => x.ListOfInts);
        DescribeEnumerable(x => x.ListOfNullableInts);
        DescribeEnumerable(x => x.NullableListOfInts);
        DescribeEnumerable(x => x.NullableListOfNullableInts);

        DescribeEnumerable(x => x.NullableListOfInts)
            .Vitally().IsNotNull()
            .ForEach(x => x.Vitally().IsNotNull());

        DescribeEnumerable(x => x.NullableListOfNullableInts)
            .Vitally().IsNotNull()
            .ForEach(x => x.Vitally().IsNotNull());

        Scope("GetSome DB Value",
            (v) => 5,
            (scoped) =>
            {
                Describe(x => x.IntField).IsNotNull();
            });

        ScopeWhen(
            "When",
            (v) => true,
            "Get 5",
            (v) => 5,
            (scoped) =>
            {
                DescribeEnumerable(x => x.NullableListOfNullableInts)
                    .IsNotNull();
            });

        When(
           "When",
           (v) => Task.FromResult(true),
           () =>
           {
               DescribeEnumerable(x => x.NullableListOfNullableInts)
                   .IsNotNull();
           });

        Switch("Get DB Value", (v) => 5)
            .Case(x => x.NullableListOfNullableInts, "This is a Test for reasons", (x, y) => x is null, "it was null");
    }
}

public class NullabilityWarningsTests
{
    [Fact]
    public void GivenAComprehensiveTestOfCode_WhenCheckedForWarnings_ItGeneratesNone()
    {
        string path = Path.Combine(
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!, 
            @"PublicAccessTests\NullabilityWarningTests_NullCheckCode.cs"
        );

        var codeToEval = File.ReadAllText(path);

        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToEval);

        string assemblyName = Path.GetRandomFileName();
        var references = new List<MetadataReference>
        {
            MetadataReference.CreateFromFile(typeof(AbstractValidator<>).Assembly.Location)
        };
        references.AddRange(GetSystemMetadataReferences());

        CSharpCompilation compilation = CSharpCompilation.Create(
            assemblyName,
            syntaxTrees: new[] { syntaxTree },
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var resultString = string.Empty;

        using (var ms = new MemoryStream())
        {
            EmitResult emitResult = compilation.Emit(ms);

            foreach (Diagnostic diagnostic in emitResult.Diagnostics)
            {
                resultString += string.Format("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                resultString += Environment.NewLine;
            }
        }

        Approvals.Verify(resultString);
    }

    public static List<MetadataReference> GetSystemMetadataReferences()
    {
        string trustedAssemblies = (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") as string)!;
        string[] trustedList = trustedAssemblies.Split(';');
        
        List<string> required = new List<string>()
        {
            "System.Runtime.dll",
            "netstandard.dll",
            "mscorlib.dll",
            "System.Private.CoreLib.dll",
            "System.Linq.Expressions.dll",
            "System.Collections.Generic.dll"
        };
        
        List<string> filteredPathList = trustedList.Where(p => required.Any(r => p.Contains(r))).ToList();
        List<MetadataReference> ret = new List<MetadataReference>();

        foreach (var path in filteredPathList)
        {
            ret.Add(MetadataReference.CreateFromFile(path));
        }

        return ret;
    }
}

