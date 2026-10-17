using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace TheOne.Avalonia.Controls;

public sealed record StepValidationError(string MemberName, string Message);

internal interface IStepValidationScope
{
    object Model { get; }
    bool IsAffectedBy(string? memberName);
}

/// <summary>Validates one step. Implement this for asynchronous or service-backed validation.</summary>
public interface IStepValidator
{
    Task<IReadOnlyList<StepValidationError>> ValidateAsync(CancellationToken cancellationToken);
}

/// <summary>
///     Creates attribute validators for concrete bound model types without dynamic binding or assembly-wide
///     preservation.
/// </summary>
public static class StepValidation
{
    public static IStepValidator For<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>(
        T model, params string[] memberNames) where T : class
    {
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(memberNames);
        if (model.GetType() != typeof(T))
            throw new ArgumentException("Use the concrete model type when creating a step validator.", nameof(model));
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(property =>
            property.GetMethod is { IsPublic: true } && property.GetIndexParameters().Length == 0).ToArray();
        foreach (var name in memberNames)
            if (!properties.Any(property => property.Name == name))
                throw new ArgumentException($"Unknown validation property '{name}'.", nameof(memberNames));
        var selected = memberNames.Length == 0
            ? properties
            : properties.Where(property => memberNames.Contains(property.Name)).ToArray();
        return new AttributeValidator<T>(model, selected, properties, memberNames.ToArray());
    }

    private sealed class AttributeValidator<T>(
        T model,
        PropertyInfo[] properties,
        PropertyInfo[] allProperties,
        string[] memberNames) : IStepValidator, IStepValidationScope where T : class
    {
        public object Model => model;

        public bool IsAffectedBy(string? memberName)
        {
            return string.IsNullOrEmpty(memberName) || memberNames.Length == 0 || memberNames.Contains(memberName)
                   || properties.SelectMany(property => property.GetCustomAttributes<StepCompareAttribute>())
                       .Any(rule => rule.OtherProperty == memberName);
        }

        public Task<IReadOnlyList<StepValidationError>> ValidateAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var errors = new List<StepValidationError>();
            foreach (var property in properties)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var rules = property.GetCustomAttributes<ValidationAttribute>(true).ToArray();
                if (rules.Length == 0) continue;
                var display = property.GetCustomAttribute<DisplayAttribute>()?.GetName() ?? property.Name;
                // The explicit display-name constructor avoids ValidationContext's runtime type discovery.
                var context = new ValidationContext(model, display, null,
                        new Dictionary<object, object?>
                            { [StepCompareAttribute.AccessorKey] = (Func<string, (bool Found, object? Value)>)Read })
                    { MemberName = property.Name };
                var value = property.GetValue(model);
                foreach (var rule in rules.OrderBy(rule => rule is RequiredAttribute ? 0 : 1))
                    if (rule.GetValidationResult(value, context) is { } result)
                    {
                        errors.Add(new StepValidationError(property.Name,
                            result.ErrorMessage ?? $"{display} is invalid."));
                        if (rule is RequiredAttribute) break;
                    }
            }

            if (model is IValidatableObject validatable)
                foreach (var result in validatable.Validate(new ValidationContext(model, typeof(T).Name, null, null)))
                {
                    var members = result.MemberNames.ToArray();
                    if (members.Length == 0)
                        errors.Add(new StepValidationError("", result.ErrorMessage ?? "The step is invalid."));
                    else
                        foreach (var member in members.Where(member =>
                                     memberNames.Length == 0 || memberNames.Contains(member)))
                            errors.Add(new StepValidationError(member, result.ErrorMessage ?? "The step is invalid."));
                }

            return Task.FromResult<IReadOnlyList<StepValidationError>>(errors.Distinct().ToArray());

            (bool Found, object? Value) Read(string name)
            {
                var property = allProperties.FirstOrDefault(candidate => candidate.Name == name);
                return property is null ? (false, null) : (true, property.GetValue(model));
            }
        }
    }
}