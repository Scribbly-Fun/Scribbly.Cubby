using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace Scribbly.Cubby;

/// <inheritdoc />
[Generator(LanguageNames.CSharp)]
public sealed class ContainerTagGenerator : IIncrementalGenerator
{
    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<(string?, string?)> property = context
            .AnalyzerConfigOptionsProvider
            .Select((options, _) =>
            {
                options.GlobalOptions.TryGetValue(
                    "build_property.CubbyContainerImageTag",
                    out var cubbyContainer);

                options.GlobalOptions.TryGetValue(
                    "build_property.PortalContainerImageTag",
                    out var portalContainer);

                return (cubbyContainer, portalContainer);
            });

        context.RegisterSourceOutput(property, (spc, value) =>
        {
            var (cubby, portal) = value;
            
            if (string.IsNullOrWhiteSpace(cubby))
                return;

            var source = $$"""
                           namespace Scribbly.Cubby;
                           
                           internal static class CubbyResources
                           {
                               public const string CubbyContainerImageTag = "{{cubby}}";
                               public const string PortalContainerImageTag = "{{portal ?? cubby}}";
                           }
                           
                           """;

            spc.AddSource(
                "CubbyResourceConstants.g.cs",
                SourceText.From(source, Encoding.UTF8));
        });
    }
}