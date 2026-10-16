global using TypeContract = TheOne.Documentation.TypeContract;
global using PropertyContract = TheOne.Documentation.PropertyContract;
global using MemberContract = TheOne.Documentation.MemberContract;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using TheOne.DesignPreview.Reference;

namespace TheOne.Documentation;

// Compile the same strings shown by PreviewHost; snippets are never invoked.
internal static class ExampleVerifier
{
    private static readonly HashSet<string> CoreOwners = new(StringComparer.Ordinal)
    {
        "ReportDesign", "ReportPreview", "WorkflowEditor", "WorkflowSerializer",
        "WorkflowDocument", "Form", "ChartBase", "MultiSelectBox"
    };

    internal static int Verify(CSharpCompilation compilation, ContractSet data)
    {
        var snippets = new List<(TypeContract Owner, MemberContract Member, string Code, SyntaxTree Tree)>();
        foreach (var owner in data.Types.Where(type => CoreOwners.Contains(type.Name)))
        foreach (var member in owner.Methods)
        {
            var code = ControlUsageExamples.Method(owner, member);
            if (code.Length == 0) continue;
            var method = MethodName(member.Signature);
            var source = $$"""
                using System;
                using System.IO;
                using System.Linq;
                using System.Threading;
                using System.Threading.Tasks;
                using Avalonia;
                using TheOne.Avalonia.Controls;
                using TheOne.Avalonia.Reporting;
                using TheOne.Avalonia.Workflow;

                internal static class UsageSnippet_{{snippets.Count}}
                {
                    internal static async Task Verify({{Context(owner.Name, method)}})
                    {
                        {{code}}
                    }
                }
                """;
            var tree = CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview),
                "usage-examples/" + owner.Name + "." + method + "." + snippets.Count + ".cs");
            snippets.Add((owner, member, code, tree));
        }
        var snippetTrees = snippets.Select(snippet => snippet.Tree).ToHashSet();
        var checkedCompilation = compilation.AddSyntaxTrees(snippetTrees);
        var errors = checkedCompilation.GetDiagnostics()
            .Where(diagnostic => diagnostic.Severity == DiagnosticSeverity.Error && diagnostic.Location.SourceTree is { } tree && snippetTrees.Contains(tree))
            .ToArray();
        foreach (var group in snippets.GroupBy(snippet => snippet.Owner.Name))
            Console.WriteLine($"Usage snippets/{group.Key}: {group.Count()} checked.");
        Console.WriteLine($"Usage snippets: {snippets.Count} business examples, {errors.Length} errors; no snippet was executed.");
        foreach (var error in errors) Console.Error.WriteLine(error);
        return errors.Length == 0 ? 0 : 1;
    }

    private static string Context(string owner, string method) => owner switch
    {
        "ReportDesign" => "ReportDesign designer" + (method == "DropDataNodeAsync" ? ", ReportDataNode dataNode" : ""),
        "ReportPreview" => "ReportPreview preview",
        "WorkflowEditor" => "WorkflowEditor editor" + (method == "AddNode" ? ", WorkflowNodeDefinition definition" : method is "SelectNode" or "Locate" ? ", WorkflowNode node" : ""),
        "WorkflowSerializer" => "WorkflowEditor editor",
        "WorkflowDocument" => "WorkflowDocument document" + (method switch
        {
            "Undo" or "Redo" or "Remove" => ", WorkflowEditor editor",
            "MoveNodes" or "Edit" => ", WorkflowNode node",
            "SetProperty" => ", WorkflowProperty property",
            "GetReferenceOptions" => ", WorkflowNode targetNode, WorkflowProperty property",
            _ => ""
        }),
        "Form" => "Form form",
        "ChartBase" => "ChartBase chart",
        "MultiSelectBox" => "MultiSelectBox multiSelect" + (method == "ToggleItem" ? ", object option" : ""),
        _ => throw new InvalidOperationException("Missing snippet context for " + owner)
    };

    private static string MethodName(string signature)
    {
        var open = signature.IndexOf('(');
        var name = signature[..open];
        name = name[(name.LastIndexOf('.') + 1)..];
        var generic = name.IndexOf('<');
        return generic < 0 ? name : name[..generic];
    }
}
