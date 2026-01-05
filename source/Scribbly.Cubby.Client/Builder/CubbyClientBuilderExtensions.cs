using System.Text.Json;
using Scribbly.Cubby.Client.Serializer;

namespace Scribbly.Cubby.Client;

/// <summary>
/// Extends the client builder options to include MessagePack serializers.
/// </summary>
public static class CubbyClientBuilderExtensions
{
    /// <summary>
    /// Extends the Cubby Client Builder.
    /// </summary>
    /// <param name="options">Cubby options</param>
    extension(CubbyClientOptions options)
    {
        /// <summary>
        /// Adds and configures the message pack serializer.
        /// </summary>
        public void AddSystemTextSerializer(
            Action<JsonSerializerOptions>? optionsCallback = null,
            ICubbyCompressor? compressor = null)
        {
            options.AddSerializer<JsonCubbySerializer>(optionsCallback, compressor);
        }
    }
}