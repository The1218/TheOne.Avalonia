using System.ComponentModel.DataAnnotations;

namespace TheOne.Avalonia.Controls;

/// <summary>Cross-field comparison using StepValidation's preserved property accessors, without runtime type discovery.</summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class StepCompareAttribute : ValidationAttribute
{
    internal const string AccessorKey = "TheOne.Step.PropertyAccessor";
    public string OtherProperty { get; }
    public StepCompareAttribute(string otherProperty) : base("{0} must match the comparison field.")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(otherProperty); OtherProperty = otherProperty;
    }
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Ordinary binding validation has no step scope. The comparison runs before advancing.
        if (!validationContext.Items.TryGetValue(AccessorKey, out var accessor) || accessor is not Func<string, (bool Found, object? Value)> read) return ValidationResult.Success;
        var other = read(OtherProperty);
        if (!other.Found) return new ValidationResult($"Unknown comparison property '{OtherProperty}'.");
        return Equals(value, other.Value) ? ValidationResult.Success : new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
    }
}
