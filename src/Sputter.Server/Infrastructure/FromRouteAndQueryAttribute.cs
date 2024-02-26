using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Sputter.Server.Infrastructure;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
public class FromRouteAndQueryAttribute : Attribute, IBindingSourceMetadata {
    public BindingSource BindingSource { get; } = CompositeBindingSource.Create(
        new[] { BindingSource.Path, BindingSource.Query }, nameof(FromRouteAndQueryAttribute));
}
