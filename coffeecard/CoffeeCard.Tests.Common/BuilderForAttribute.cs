namespace CoffeeCard.Tests.Common;

/// <summary>
/// Serves as a marker attribute used for source generation
/// The Type given, will have builder methods generated for it
/// The methods will be in the format WithPropertyName and allow for fluent api style configuration
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
sealed class BuilderForAttribute : Attribute
{
    public Type EntityType { get; }

    public BuilderForAttribute(Type entityType)
    {
        EntityType = entityType;
    }
}
