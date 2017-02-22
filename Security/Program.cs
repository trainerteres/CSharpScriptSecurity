using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security
{
    class Program
    {
        static void Main(string[] args)
        {
            var code = File.ReadAllText("F:\\SkyDrive\\Robin's Community Work\\Vorträge\\C# Script\\Scripts\\SecurityDemo_02.csx");
            var script = CSharpScript.Create(code);

            var diagnostics = script.Compile();
            if (diagnostics.Count() > 0)
            {
                foreach (var diagnostic in diagnostics)
                    Console.WriteLine(diagnostic.ToString());
            }

            var securityDiagnostics = CheckSecurity(script);
            if (securityDiagnostics.Count() > 0)
            {
                foreach (var diagnostic in securityDiagnostics)
                    Console.WriteLine(diagnostic.ToString());

                Console.ReadLine();
                return;
            }

            Console.WriteLine("The script is secure and will be executed...");

            script.RunAsync();

            Console.ReadLine();
        }

        private static ImmutableArray<Diagnostic> CheckSecurity(Script<object> script)
        {
            var compilation = script.GetCompilation();
            var tree = compilation.SyntaxTrees.FirstOrDefault();

            if (tree == null)
                return ImmutableArray.Create<Diagnostic>();

            var semanticModel = compilation.GetSemanticModel(tree);
            var treeRoot = tree.GetRoot() as CompilationUnitSyntax;
            var identifiers = treeRoot.DescendantNodes().OfType<IdentifierNameSyntax>();

            var diagnostics = new List<Diagnostic>();

            foreach (var identifier in identifiers)
            {
                if (identifier.IsVar)
                    continue;

                var typeInfo = semanticModel.GetTypeInfo(identifier);

                if (typeInfo.Type != null && 
                    typeInfo.Type.ContainingNamespace != null)
                {
                    if (typeInfo.Type.ContainingNamespace.ToString() == "System.IO")
                    {
                        var descriptor =
                            new DiagnosticDescriptor(
                                "SSV01",
                                "Security Breach",
                                "Use of types from namespace System.IO is not allowed.",
                                "Usage",
                                DiagnosticSeverity.Error,
                                true);

                        diagnostics.Add(Diagnostic.Create(descriptor, identifier.GetLocation(), typeInfo.Type.ContainingNamespace.Name));
                    }
                }
                else
                {
                    var symbolInfo = semanticModel.GetSymbolInfo(identifier);

                    if (symbolInfo.Symbol != null)
                    {
                        var namedType = symbolInfo.Symbol as INamedTypeSymbol;

                        if (namedType != null && 
                            namedType.ContainingNamespace != null && 
                            namedType.ContainingNamespace.ToString() == "System.IO")
                        {
                            var descriptor =
                                new DiagnosticDescriptor(
                                    "SSV01",
                                    "Security Breach",
                                    "Use of types from namespace System.IO is not allowed.",
                                    "Usage",
                                    DiagnosticSeverity.Error,
                                    true);

                            diagnostics.Add(Diagnostic.Create(descriptor, identifier.GetLocation(), typeInfo.Type.ContainingNamespace.Name));
                        }
                    }
                }
            }

            return diagnostics.ToImmutableArray();
        }

    }
}
