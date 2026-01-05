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
        IncrementalValueProvider<string?> property = context
            .AnalyzerConfigOptionsProvider
            .Select((options, _) =>
            {
                options.GlobalOptions.TryGetValue(
                    "build_property.CubbyContainerImageTag",
                    out var value);

                return value;
            });

        context.RegisterSourceOutput(property, (spc, value) =>
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            var source = $$"""
                           namespace Scribbly.Cubby
                           {
                               internal static class BuildConstants
                               {
                                   public const string ContainerImageTag = "{{value}}";
                               }
                           }
                           """;

            spc.AddSource(
                "BuildConstants.g.cs",
                SourceText.From(source, Encoding.UTF8));
        });
    }
}