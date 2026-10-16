using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TheOne.Documentation;

internal static class Program
{
    private static readonly SymbolDisplayFormat Display = SymbolDisplayFormat.FullyQualifiedFormat.WithMiscellaneousOptions(
        SymbolDisplayMiscellaneousOptions.UseSpecialTypes | SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier);

    public static int Main(string[] args)
    {
        if (args.Length < 3) throw new ArgumentException("Arguments: repository-root library-project.assets.json output.json [--generate|--check]");
        var root = Path.GetFullPath(args[0]);
        var files = Directory.GetFiles(Path.Combine(root, "TheOne.Avalonia"), "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Split(Path.DirectorySeparatorChar).Any(s => s is "bin" or "obj")).OrderBy(p => p, StringComparer.Ordinal);
        var trees = files.Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), new CSharpParseOptions(LanguageVersion.Preview, DocumentationMode.Diagnose), p)).ToList();
        var vendorFiles = new HashSet<string>(StringComparer.Ordinal) { "DataGridRowMetadata.cs", "DataGridCollectionView.Generic.cs", "DataGridTreeCollectionView.cs", "DataGridTreeCollectionView.Query.cs", "DataGridColumn.cs" };
        trees.AddRange(Directory.GetFiles(Path.Combine(root, "vendor/Avalonia.Controls.DataGrid"), "*.cs", SearchOption.AllDirectories)
            .Where(p => !p.Split(Path.DirectorySeparatorChar).Any(s => s is "bin" or "obj")).Order(StringComparer.Ordinal)
            .Select(p => CSharpSyntaxTree.ParseText(File.ReadAllText(p), new CSharpParseOptions(LanguageVersion.Preview, DocumentationMode.Diagnose), p)));
        trees.Add(CSharpSyntaxTree.ParseText("global using System; global using System.Collections.Generic; global using System.Linq; global using System.Threading; global using System.Threading.Tasks; global using System.IO; global using System.Net.Http;", new CSharpParseOptions(LanguageVersion.Preview)));
        var references = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!).Split(Path.PathSeparator).ToHashSet(StringComparer.Ordinal);
        using var assets = JsonDocument.Parse(File.ReadAllText(args[1]));
        var folders = assets.RootElement.GetProperty("packageFolders").EnumerateObject().Select(p => p.Name).ToArray();
        foreach (var library in assets.RootElement.GetProperty("targets").EnumerateObject().First().Value.EnumerateObject())
        {
            if (!library.Value.TryGetProperty("compile", out var compile)) continue;
            foreach (var file in compile.EnumerateObject().Where(p => p.Name.EndsWith(".dll", StringComparison.Ordinal)))
            foreach (var folder in folders)
            {
                var path = Path.Combine(folder, library.Name.ToLowerInvariant(), file.Name);
                if (File.Exists(path)) references.Add(path);
            }
        }
        var compilation = CSharpCompilation.Create("TheOne.Documentation.Analysis", trees,
            references.Select(p => MetadataReference.CreateFromFile(p)), new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, nullableContextOptions: NullableContextOptions.Enable));
        var symbols = new Dictionary<string, INamedTypeSymbol>();
        foreach (var tree in trees.Where(t => t.FilePath.Length > 0 && (!t.FilePath.Contains("/vendor/", StringComparison.Ordinal) || vendorFiles.Contains(Path.GetFileName(t.FilePath)))))
        {
            var model = compilation.GetSemanticModel(tree);
            foreach (var declaration in tree.GetRoot().DescendantNodes().OfType<BaseTypeDeclarationSyntax>())
                if (model.GetDeclaredSymbol(declaration) is INamedTypeSymbol symbol && Exported(symbol)) symbols[Key(symbol)] = symbol;
            foreach (var declaration in tree.GetRoot().DescendantNodes().OfType<DelegateDeclarationSyntax>())
                if (model.GetDeclaredSymbol(declaration) is INamedTypeSymbol symbol && Exported(symbol)) symbols[Key(symbol)] = symbol;
        }
        var types = symbols.Values.OrderBy(Key, StringComparer.Ordinal).Select(s =>
        {
            var type = ReadType(s, root);
            return type.Id == "Avalonia.Controls.DataGridColumn" ? type with
            {
                Properties = type.Properties.Where(p => p.Name == "SortPriority").ToArray(),
                Methods = type.Methods.Where(m => m.Signature.Contains(".CycleSort(", StringComparison.Ordinal)).ToArray(),
                Events = [], Fields = [], Related = [], Summary = "TheOne additions to the native column; inherited/upstream APIs are omitted."
            } : type;
        }).ToArray();
        var styles = ReadStyles(root).ToArray();
        // Models flowing through object/Content or host adapters are not visible in
        // property signatures. Keep these explicit relationships in the shared inventory.
        var modelHints = new Dictionary<string, string[]>
        {
            ["RelationGraph"] = ["RelationGraphNode"],
            ["FlowStep"] = ["FlowNodeContent", "FlowNodeItem", "FlowNodeStatus"],
            ["FlowNode"] = ["FlowNodeContent", "FlowNodeItem", "FlowNodeStatus"],
            ["VerificationStep"] = ["StepItem", "StepState", "StepValidation", "StepCompareAttribute"],
            ["Form"] = ["StepValidation", "StepCompareAttribute"],
            ["Upload"] = ["UploadHttp", "UploadHttpResult", "UploadModel"],
            ["DataGridFeatures"] = ["DataGridFilterCondition", "DataGridSummaryResult", "DataGridFilterApplyResult"],
            ["DataGridTreeFeatures"] = ["DataGridTreeCollectionView", "IDataGridTreeQueryView", "DataGridTreeNodeState"]
        };
        types = types.Select(t => modelHints.TryGetValue(t.Name, out var hints)
            ? t with { Related = t.Related.Concat(types.Where(model => hints.Contains(model.Name)).Select(model => model.Id)).Distinct().Order(StringComparer.Ordinal).ToArray() }
            : t).ToArray();
        var data = new ContractSet(types, styles);
        File.WriteAllText(args[2], JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine($"Exported {types.Length} public types, {types.Sum(t => t.Properties.Length)} properties, {types.Sum(t => t.Methods.Length)} methods/constructors, {styles.Length} style-class contracts.");
        var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error && d.Id is "CS0246" or "CS0234" or "CS0012").ToArray();
        if (errors.Length > 0) { Console.Error.WriteLine(string.Join(Environment.NewLine, errors.Take(8))); return 1; }
        if (args.Contains("--generate") || args.Contains("--check"))
        {
            var result = ReferenceWriter.Write(root, data, args.Contains("--check"));
            return Math.Max(result, AccessorWriter.Write(root, data, args.Contains("--check")));
        }
        return 0;
    }

    private static bool Exported(INamedTypeSymbol type) => type.DeclaredAccessibility == Accessibility.Public &&
        (type.ContainingType is null || Exported(type.ContainingType));
    private static string Key(ITypeSymbol type) => type.ToDisplayString(Display).Replace("global::", "");
    private static string Doc(ISymbol symbol)
    {
        var xml = symbol.GetDocumentationCommentXml();
        if (string.IsNullOrWhiteSpace(xml)) return "";
        try { return Regex.Replace(XElement.Parse(xml).Element("summary")?.Value ?? "", @"\s+", " ").Trim(); }
        catch (System.Xml.XmlException) { return ""; }
    }
    private static string Location(ISymbol symbol, string root) => symbol.Locations.FirstOrDefault(l => l.IsInSource) is { } l
        ? Path.GetRelativePath(root, l.SourceTree!.FilePath).Replace('\\', '/') + ":" + (l.GetLineSpan().StartLinePosition.Line + 1) : "";
    private static string Signature(ISymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat
        .WithMemberOptions(SymbolDisplayMemberOptions.IncludeType | SymbolDisplayMemberOptions.IncludeParameters | SymbolDisplayMemberOptions.IncludeContainingType | SymbolDisplayMemberOptions.IncludeModifiers)
        .WithParameterOptions(SymbolDisplayParameterOptions.IncludeType | SymbolDisplayParameterOptions.IncludeName | SymbolDisplayParameterOptions.IncludeDefaultValue | SymbolDisplayParameterOptions.IncludeParamsRefOut)
        .WithGenericsOptions(SymbolDisplayGenericsOptions.IncludeTypeParameters | SymbolDisplayGenericsOptions.IncludeTypeConstraints | SymbolDisplayGenericsOptions.IncludeVariance));

    private static TypeContract ReadType(INamedTypeSymbol type, string root)
    {
        var members = type.GetMembers().Where(m => m.DeclaredAccessibility == Accessibility.Public && !m.IsImplicitlyDeclared).ToArray();
        var fields = members.OfType<IFieldSymbol>().ToDictionary(f => f.Name);
        var properties = new List<PropertyContract>();
        foreach (var p in members.OfType<IPropertySymbol>())
        {
            fields.TryGetValue(p.Name + "Property", out var field);
            properties.Add(ReadProperty(p.IsIndexer ? Signature(p) : p.Name, p.Type, p, field, p.SetMethod?.DeclaredAccessibility == Accessibility.Public, false, root));
        }
        foreach (var field in fields.Values.Where(f => f.Type.Name == "AttachedProperty"))
        {
            var name = field.Name.EndsWith("Property", StringComparison.Ordinal) ? field.Name[..^8] : field.Name;
            if (properties.Any(p => p.Name == name)) continue;
            var writable = type.GetMembers("Set" + name).OfType<IMethodSymbol>().Any(m => m.DeclaredAccessibility == Accessibility.Public && m.Parameters.Length == 2);
            properties.Add(ReadProperty(name, ((INamedTypeSymbol)field.Type).TypeArguments[0], field, field, writable, true, root));
        }
        var control = false;
        for (var b = type; b is not null; b = b.BaseType) if (b.ToDisplayString() == "Avalonia.Controls.Control") { control = true; break; }
        var enums = type.TypeKind == TypeKind.Enum ? fields.Values.Where(f => f.HasConstantValue).Select(f => new MemberContract(f.Name + " = " + f.ConstantValue, Doc(f), Location(f, root))).ToArray() : [];
        var related = new HashSet<string>();
        void Reference(ITypeSymbol symbol)
        {
            if (symbol is IArrayTypeSymbol array) { Reference(array.ElementType); return; }
            if (symbol is not INamedTypeSymbol named) return;
            if (named.ContainingNamespace.ToDisplayString().StartsWith("TheOne.Avalonia", StringComparison.Ordinal)
                || named.Name is "DataGridRowMetadata" or "DataGridTreeCollectionView" or "IDataGridTreeCollectionView" or "IDataGridTreeQueryView" or "DataGridTreeNodeState" or "DataGridTreeChangingEventArgs"
                || named.Name == "DataGridCollectionView" && named.IsGenericType) related.Add(Key(named.OriginalDefinition));
            foreach (var argument in named.TypeArguments) Reference(argument);
        }
        foreach (var property in members.OfType<IPropertySymbol>()) Reference(property.Type);
        foreach (var method in members.OfType<IMethodSymbol>()) { Reference(method.ReturnType); foreach (var p in method.Parameters) Reference(p.Type); }
        foreach (var evt in members.OfType<IEventSymbol>()) Reference(evt.Type);
        return new TypeContract(Key(type), type.Name, type.ContainingNamespace.ToDisplayString(), type.TypeKind.ToString(), control,
            type.IsStatic, type.BaseType is null ? "" : Key(type.BaseType), Doc(type), Location(type, root),
            properties.OrderBy(p => p.Name, StringComparer.Ordinal).ToArray(),
            members.OfType<IMethodSymbol>().Where(m => m.MethodKind == MethodKind.Ordinary).Concat(type.InstanceConstructors.Where(m => m.DeclaredAccessibility == Accessibility.Public))
                .Select(m => new MemberContract(Signature(m), Doc(m), Location(m, root).Length == 0 ? Location(type, root) : Location(m, root))).ToArray(),
            members.OfType<IEventSymbol>().Select(e => new MemberContract(Signature(e), Doc(e), Location(e, root), Key(e.Type))).ToArray(),
            enums, related.Where(k => k != Key(type)).Order(StringComparer.Ordinal).ToArray(),
            members.OfType<IFieldSymbol>().Where(f => type.TypeKind != TypeKind.Enum).Select(f => new MemberContract(Signature(f), Doc(f), Location(f, root))).ToArray());
    }

    private static PropertyContract ReadProperty(string name, ITypeSymbol valueType, ISymbol member, IFieldSymbol? field, bool writable, bool attached, string root)
    {
        var initializer = field?.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() is VariableDeclaratorSyntax v ? v.Initializer?.Value : null;
        var registration = initializer?.ToString() ?? "";
        var mode = Regex.Match(registration, @"defaultBindingMode:\s*(?:global::)?(?:\w+\.)*(\w+)").Groups[1].Value;
        var validation = initializer is InvocationExpressionSyntax registrationCall
            ? registrationCall.ArgumentList.Arguments.FirstOrDefault(a => a.NameColon?.Name.Identifier.Text == "validate")?.Expression.ToString() ?? "" : "";
        var defaultValue = valueType.IsReferenceType || valueType.NullableAnnotation == NullableAnnotation.Annotated ? "null" : valueType.SpecialType == SpecialType.System_Boolean ? "false" : "0 / default";
        if (initializer is InvocationExpressionSyntax invoke)
        {
            var explicitDefault = invoke.ArgumentList.Arguments.FirstOrDefault(a => a.NameColon?.Name.Identifier.Text == "defaultValue");
            if (explicitDefault is not null) defaultValue = explicitDefault.Expression.ToString();
            else if (registration.Contains("RegisterDirect")) defaultValue = "computed / state";
            else if (registration.Contains("AddOwner")) defaultValue = "inherited registration";
            else if (invoke.ArgumentList.Arguments.Count > 1 && invoke.ArgumentList.Arguments[1].NameColon is null) defaultValue = invoke.ArgumentList.Arguments[1].Expression.ToString();
        }
        if (field is null)
        {
            var syntax = member.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
            if (syntax is ParameterSyntax parameter) defaultValue = parameter.Default?.Value.ToString() ?? "required constructor argument";
            else if (syntax is PropertyDeclarationSyntax p)
            {
                if (p.Initializer is not null) defaultValue = p.Initializer.Value.ToString();
                else
                {
                    var getExpression = p.ExpressionBody?.Expression ?? p.AccessorList?.Accessors.FirstOrDefault(a => a.IsKind(SyntaxKind.GetAccessorDeclaration))?.ExpressionBody?.Expression;
                    if (getExpression is IdentifierNameSyntax identifier && member.ContainingType.GetMembers(identifier.Identifier.Text).OfType<IFieldSymbol>().FirstOrDefault() is { } backing)
                        defaultValue = (backing.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax() as VariableDeclaratorSyntax)?.Initializer?.Value.ToString() ?? defaultValue;
                    else if (getExpression is not null || !writable) defaultValue = "accessor-defined / state";
                }
            }
        }
        var enumType = valueType as INamedTypeSymbol;
        if (enumType?.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T) enumType = enumType.TypeArguments[0] as INamedTypeSymbol;
        var binding = mode.Length > 0 ? mode : field is not null ? "OneWay" : member is IPropertySymbol { SetMethod.IsInitOnly: true } ? "CLR / init-only" : "CLR / n/a";
        return new PropertyContract(name, Key(valueType), Doc(member).Length > 0 ? Doc(member) : field is not null ? Doc(field) : "",
            defaultValue, binding, validation, writable, attached, field?.Name ?? "", Location(member, root),
            registration.Contains("inherits: true"), member.IsStatic, enumType?.TypeKind == TypeKind.Enum ? enumType.GetMembers().OfType<IFieldSymbol>().Where(f => f.HasConstantValue).Select(f => f.Name).ToArray() : [],
            attached ? member.ContainingType.GetMembers("Get" + name).OfType<IMethodSymbol>().FirstOrDefault(m => m.DeclaredAccessibility == Accessibility.Public && m.Parameters.Length == 1) is { } getter ? Key(getter.Parameters[0].Type) : "" : member.ContainingType.Name == "DataGridColumn" ? Key(member.ContainingType) : "");
    }

    private static IEnumerable<StyleContract> ReadStyles(string root)
    {
        var found = new Dictionary<(string, string), StyleContract>();
        foreach (var file in Directory.GetFiles(Path.Combine(root, "TheOne.Avalonia/Themes"), "*.axaml", SearchOption.AllDirectories).Order(StringComparer.Ordinal))
        {
            var doc = XDocument.Load(file, LoadOptions.SetLineInfo);
            foreach (var style in doc.Descendants().Where(e => e.Name.LocalName == "Style" && e.Attribute("Selector") is not null))
            {
                var selector = style.Attribute("Selector")!.Value;
                var theme = style.Ancestors().FirstOrDefault(e => e.Name.LocalName == "ControlTheme");
                var fallback = theme?.Attribute("TargetType")?.Value.Split(':').Last() ?? "";
                foreach (Match match in Regex.Matches(selector, @"(?:(?::is\((?<type>[\w|]+)\)|(?<type>[A-Za-z][\w|]*))(?:#[\w-]+)?)?\.(?<class>[A-Za-z][\w-]*)"))
                {
                    var owner = match.Groups["type"].Value.Split('|').Last(); if (owner.Length == 0) owner = fallback;
                    if (owner.Length == 0) continue;
                    var name = match.Groups["class"].Value;
                    var setters = string.Join("; ", style.Elements().Where(e => e.Name.LocalName == "Setter").Select(e => (e.Attribute("Property")?.Value ?? "") + "=" + (e.Attribute("Value")?.Value ?? "template/resource")));
                    var key = (owner, name); var source = Path.GetRelativePath(root, file).Replace('\\', '/') + ":" + ((System.Xml.IXmlLineInfo)style).LineNumber;
                    var internalClass = name.StartsWith("one-", StringComparison.Ordinal) || name.StartsWith("workflow-", StringComparison.Ordinal)
                        || name.StartsWith("pagination-", StringComparison.Ordinal) || name.StartsWith("flow-", StringComparison.Ordinal)
                        || name is "reached" or "joined" or "card-tab" or "last-tab" or "before-selected" or "step-indicator"
                        || style.Ancestors().Any(e => e.Name.LocalName is "DataTemplate" or "ControlTemplate");
                    var effect = setters.Length > 0 ? selector + " => " + setters : "";
                    if (found.TryGetValue(key, out var current)) found[key] = current with { Effect = string.Join("; ", new[] { current.Effect, effect }.Where(value => value.Length > 0)), Internal = current.Internal && internalClass };
                    else found[key] = new StyleContract(owner, name, selector, effect, internalClass, source);
                }
            }
        }
        return found.Values.OrderBy(s => s.Target, StringComparer.Ordinal).ThenBy(s => s.Name, StringComparer.Ordinal);
    }
}

internal sealed record ContractSet(TypeContract[] Types, StyleContract[] Styles);
internal sealed record TypeContract(string Id, string Name, string Namespace, string Kind, bool IsControl, bool IsStatic, string Base,
    string Summary, string Source, PropertyContract[] Properties, MemberContract[] Methods, MemberContract[] Events, MemberContract[] Values, string[] Related, MemberContract[] Fields);
internal sealed record PropertyContract(string Name, string Type, string Summary, string Default, string Binding, string Validation,
    bool Writable, bool Attached, string Identifier, string Source, bool Inherits, bool Static, string[] Choices, string Target);
internal sealed record MemberContract(string Signature, string Summary, string Source, string ValueType = "");
internal sealed record StyleContract(string Target, string Name, string Selector, string Effect, bool Internal, string Source);
