using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using global::Avalonia;
using global::Avalonia.Automation;
using global::Avalonia.Controls;
using global::Avalonia.Controls.Templates;
using global::Avalonia.Data;
using global::Avalonia.Data.Core;
using global::Avalonia.Layout;
using global::Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;
using global::Avalonia.Styling;

namespace TheOne.Avalonia.Controls;

internal sealed class StepFormProperty(object model, PropertyInfo property)
{
    public object Model { get; } = model;
    public PropertyInfo Property { get; } = property;
    public DisplayAttribute? Display { get; } = property.GetCustomAttribute<DisplayAttribute>();
    public IReadOnlyList<StepValidationError> Errors { get; set; } = [];
    public IReadOnlyList<StepValidationError> DraftErrors { get; set; } = [];
    public string? DraftText { get; set; }
}

/// <summary>Library-owned, lazy form generation. No runtime binding paths or consumer validation adapters.</summary>
internal static class StepForm
{
    public static IReadOnlyList<StepFormProperty> Properties<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(T model) where T : class
        => typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(property => property.GetMethod is { IsPublic: true } && property.SetMethod is { IsPublic: true }
                && property.GetIndexParameters().Length == 0 && property.GetCustomAttribute<DisplayAttribute>()?.GetAutoGenerateField() != false)
            .Select(property => new StepFormProperty(model, property)).OrderBy(field => field.Display?.GetOrder() ?? 10000).ToArray();

    public static Control Build(Step owner, IReadOnlyList<StepFormProperty> properties)
    {
        var form = new ListBox { Name = "PART_FormFields", ItemsSource = properties };
        if (owner.TryFindResource("One.StepForm", owner.ActualThemeVariant, out var theme)) form.Theme = (ControlTheme?)theme;
        form.ItemTemplate = new FuncDataTemplate<StepFormProperty>((field, _) => field is null ? null : Editor(field));
        return form;
    }

    private static Control Editor(StepFormProperty field)
    {
        var property = field.Property;
        var title = field.Display?.GetName() ?? property.Name;
        var kind = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
        var readOnly = property.GetCustomAttribute<EditableAttribute>()?.AllowEdit == false;
        var accessor = new ClrPropertyInfo(property.Name, property.GetValue, readOnly ? null : property.SetValue, property.PropertyType);
        var path = new CompiledBindingPathBuilder().Property(accessor, PropertyInfoAccessorFactory.CreateInpcPropertyAccessor).Build();
        var binding = new CompiledBinding(path) { Source = field.Model, Mode = readOnly ? BindingMode.OneWay : BindingMode.TwoWay };
        Control input;
        if (kind == typeof(bool))
        {
            var check = new CheckBox { Content = title, IsEnabled = !readOnly, IsThreeState = Nullable.GetUnderlyingType(property.PropertyType) is not null };
            check.Bind(global::Avalonia.Controls.Primitives.ToggleButton.IsCheckedProperty, binding);
            input = check;
        }
        else
        {
            var dataType = property.GetCustomAttribute<DataTypeAttribute>()?.DataType;
            var text = new TextBox
            {
                IsReadOnly = readOnly || !TextType(kind), PlaceholderText = field.Display?.GetPrompt(),
                PasswordChar = dataType == DataType.Password ? '●' : '\0', AcceptsReturn = dataType == DataType.MultilineText
            };
            text.Bind(TextBox.TextProperty, binding);
            // Preserve invalid editor drafts across container recycling: the model retains its last valid value.
            text.PropertyChanged += (_, change) =>
            {
                if (change.Property != DataValidationErrors.ErrorsProperty) return;
                var errors = (DataValidationErrors.GetErrors(text) ?? []).ToArray();
                field.DraftText = errors.Length > 0 ? text.Text : null;
                field.DraftErrors = errors.Select(error => new StepValidationError(property.Name,
                    error is Exception exception ? exception.GetBaseException().Message : error.ToString() ?? "Invalid input.")).ToArray();
            };
            if (field.DraftText is { } draft) text.Text = draft;
            input = text;
        }
        input.Name = property.Name;
        AutomationProperties.SetName(input, title);
        var wrapper = new StepField { MemberName = property.Name, Content = input };
        wrapper.SetErrors(field.Errors);
        var panel = new StackPanel { Spacing = 6, Margin = new Thickness(0, 0, 0, 12) };
        if (kind != typeof(bool)) panel.Children.Add(new TextBlock { Text = title });
        panel.Children.Add(wrapper);
        if (field.Display?.GetDescription() is { Length: > 0 } description)
            panel.Children.Add(new TextBlock { Text = description, TextWrapping = global::Avalonia.Media.TextWrapping.Wrap });
        return panel;
    }

    private static bool TextType(Type type) => type == typeof(string) || type == typeof(char) || type == typeof(byte) || type == typeof(sbyte)
        || type == typeof(short) || type == typeof(ushort) || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong)
        || type == typeof(float) || type == typeof(double) || type == typeof(decimal) || type == typeof(Guid)
        || type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan);
}
