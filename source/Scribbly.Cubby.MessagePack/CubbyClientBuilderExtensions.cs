using Nerdbank.MessagePack;
using PolyType;
using Scribbly.Cubby.Client;

namespace Scribbly.Cubby.MessagePack;

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
        public void AddMessagePackSerializer(ITypeShapeProvider shapeProvider)
        {
            var serializer = new MessagePackCubbySerializer(new MessagePackSerializer(), shapeProvider);
            options.AddSerializer(serializer);
        }
    }
}