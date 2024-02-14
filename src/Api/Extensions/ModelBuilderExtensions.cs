using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

public static class ModelBuilderExtensions
{
    public static void ApplyAllConfigurationsFromCurrentAssembly(
        this ModelBuilder modelBuilder,
        Assembly? assembly = null,
        string configNamespace = "")
    {
        assembly ??= Assembly.GetCallingAssembly();
        modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
}