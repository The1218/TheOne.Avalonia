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
        var propertyRows = new List<Func<DescriptionItem>>();
        foreach (var owner in owners)
        foreach (var property in ContractCatalog.ControlProperties(owner))
            propertyRows.Add(() => PropertyRow(owner, property));

        if (propertyRows.Count > 0)
            body.Children.Add(PreviewSection.Create(name + " · 额外属性", Rows(propertyRows, "AdditionalProperties", 220)));

        var apiRows = new List<Func<DescriptionItem>>();
        foreach (var owner in ContractCatalog.BusinessOwners(name))
        foreach (var method in ContractCatalog.BusinessMethods(owner))
            apiRows.Add(() => new DescriptionItem
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
        if (apiRows.Count > 0)
            body.Children.Add(PreviewSection.Create(name + " · 业务 API", Rows(apiRows, "BusinessApi", 220)));

        var modelBody = new StackPanel { Spacing = 16 };
        var ownerIds = owners.Select(owner => owner.Id).ToHashSet();
        foreach (var model in ContractCatalog.Models(owners)
                     .Where(model => !ownerIds.Contains(model.Id) && !model.IsStatic))
        {
            var modelRows = new List<Func<DescriptionItem>>();
            foreach (var property in ContractCatalog.BindableProperties(model))
                modelRows.Add(() => Row(property.Name, property.Type, property.Summary,
                    "默认：" + property.Default));
            if (modelRows.Count == 0) continue;
            modelBody.Children.Add(new StackPanel
                { Spacing = 8, Children = { Text(model.Name, true), Rows(modelRows, model.Name, 180) } });
        }

        if (modelBody.Children.Count > 0) body.Children.Add(PreviewSection.Create(name + " · Model", modelBody));
        Content = body;
        IsVisible = body.Children.Count > 0;
    }

    internal string ControlName { get; }

    internal static PreviewDeferredContent? CreateDeferred(string name)
    {
        var owners = ContractCatalog.Owners(name);
        var ownerIds = owners.Select(owner => owner.Id).ToHashSet();
        var hasRows = owners.Any(owner => ContractCatalog.ControlProperties(owner).Any()) ||
                      ContractCatalog.BusinessOwners(name).Any(owner => ContractCatalog.BusinessMethods(owner).Any()) ||
                      ContractCatalog.Models(owners).Any(model => !ownerIds.Contains(model.Id) && !model.IsStatic &&
                          ContractCatalog.BindableProperties(model).Any());
        return hasRows ? new PreviewDeferredContent(() => new ControlDetailsView(name), 320, keyboardEntry: true) : null;
    }
    private static StackPanel Rows(List<Func<DescriptionItem>> rows, string name, double labelWidth)
    {
        var body = new StackPanel { Spacing = 8 };
        foreach (var batch in rows.Chunk(6))
            body.Children.Add(new PreviewDeferredContent(() =>
            {
                var table = new Description { Name = name, LabelWidth = labelWidth };
                foreach (var row in batch) table.Items.Add(row());
                return table;
            }, batch.Length * 120, keyboardEntry: name is "AdditionalProperties" or "BusinessApi"));
        return body;
    }

    private static DescriptionItem PropertyRow(TypeContract owner, PropertyContract property)
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
        return new DescriptionItem { Header = owner.Name + "." + property.Name, Content = content };
    }

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
