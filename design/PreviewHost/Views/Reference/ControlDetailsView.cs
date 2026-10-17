using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using TheOne.Avalonia.Controls;
using TheOne.DesignPreview.Reference;

namespace TheOne.DesignPreview;

// Usage documentation is static: it does not observe or mutate the example's state.
internal sealed class ControlDetailsView : UserControl
{
    internal ControlDetailsView(string name)
    {
        ControlName = name;
        var owners = ContractCatalog.Owners(name);
        var body = new StackPanel { Spacing = 24 };
        foreach (var owner in owners)
        foreach (var property in ContractCatalog.ControlProperties(owner))
        {
            var content = new StackPanel
            {
                Spacing = 6, Children =
                {
                    Text(property.Type + (property.Writable ? "" : " · 只读"), true), Text(property.Summary),
                    Text("默认：" + property.Default + " · 绑定：" + property.Binding)
                }
            };
            if (property.Choices.Length > 0) content.Children.Add(Text("可选值：" + string.Join(" / ", property.Choices)));
            if (property.Validation.Length > 0) content.Children.Add(Text("校验：" + property.Validation));
            content.Children.Add(Code(ControlUsageExamples.Property(owner, property), "PropertyExample"));
            PropertyTable.Items.Add(
                new DescriptionItem { Header = owner.Name + "." + property.Name, Content = content });
        }

        if (PropertyTable.ItemCount > 0) body.Children.Add(PreviewSection.Create(name + " · 额外属性", PropertyTable));

        foreach (var owner in ContractCatalog.BusinessOwners(name))
        foreach (var method in ContractCatalog.BusinessMethods(owner))
            ApiTable.Items.Add(new DescriptionItem
            {
                Header = owner.Name + "." + ContractCatalog.MethodName(method),
                Content = new StackPanel
                {
                    Spacing = 6, Children =
                    {
                        Text(method.Summary), Code(ControlUsageExamples.Method(owner, method), "BusinessApiExample")
                    }
                }
            });
        if (ApiTable.ItemCount > 0) body.Children.Add(PreviewSection.Create(name + " · 业务 API", ApiTable));

        var modelTables = new List<Description>();
        var modelBody = new StackPanel { Spacing = 16 };
        var ownerIds = owners.Select(owner => owner.Id).ToHashSet();
        foreach (var model in ContractCatalog.Models(owners)
                     .Where(model => !ownerIds.Contains(model.Id) && !model.IsStatic))
        {
            var table = new Description();
            table.Items.Add(Row("类型", model.Id, model.Summary));
            foreach (var property in model.Properties)
                table.Items.Add(Row(property.Name, property.Type + (property.Writable ? "" : " · 只读"), property.Summary,
                    "默认：" + property.Default));
            foreach (var value in model.Values) table.Items.Add(Row("枚举值", value.Signature, value.Summary));
            // Model tables describe data, rather than repeating constructors and infrastructure APIs.
            foreach (var field in model.Fields.Where(field =>
                         !field.Signature.Contains("Property<", StringComparison.Ordinal) &&
                         !field.Signature.Contains("RoutedEvent<", StringComparison.Ordinal)))
                table.Items.Add(Row("字段", field.Signature, field.Summary));
            modelBody.Children.Add(new StackPanel { Spacing = 8, Children = { Text(model.Name, true), table } });
            modelTables.Add(table);
        }

        ModelTables = modelTables;
        if (modelTables.Count > 0) body.Children.Add(PreviewSection.Create(name + " · Model", modelBody));
        Content = body;
        IsVisible = body.Children.Count > 0;
    }

    internal string ControlName { get; }
    internal Description PropertyTable { get; } = new() { Name = "AdditionalProperties", LabelWidth = 220 };
    internal Description ApiTable { get; } = new() { Name = "BusinessApi", LabelWidth = 220 };
    internal IReadOnlyList<Description> ModelTables { get; }

    private static DescriptionItem Row(string label, params string[] lines)
    {
        return new DescriptionItem
            { Header = label, Content = Text(string.Join("\n", lines.Where(line => line.Length > 0))) };
    }

    // Short selectable snippets avoid creating a full editor for every property row.
    private static Control Code(string code, string name)
    {
        var block = new SelectableTextBlock
        {
            Name = name, Text = code, FontFamily = new FontFamily("Menlo, Consolas, monospace"), FontSize = 12,
            TextWrapping = TextWrapping.Wrap
        };
        var border = new Border { Padding = new Thickness(8), CornerRadius = new CornerRadius(4), Child = block };
        border.Bind(Border.BackgroundProperty, new DynamicResourceExtension("One.Subtle"));
        return border;
    }

    private static TextBlock Text(string text, bool heading = false)
    {
        return new TextBlock
        {
            Text = text, FontWeight = heading ? FontWeight.SemiBold : FontWeight.Normal,
            TextWrapping = TextWrapping.Wrap
        };
    }
}