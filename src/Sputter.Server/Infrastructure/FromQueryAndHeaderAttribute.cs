using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Resources;

namespace Sputter.Server.Infrastructure;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
[Obsolete("This doesn't work like you'd think it would 😢", true)]
public class FromQueryAndHeaderAttribute : Attribute, IBindingSourceMetadata {
    public BindingSource BindingSource { get; } = CompositeBindingSource.Create(
        new[] { BindingSource.Query, new BindingSource(
            "Header",
        "Header",
        isGreedy: false,
        isFromRequest: true) }, nameof(FromQueryAndHeaderAttribute));
}