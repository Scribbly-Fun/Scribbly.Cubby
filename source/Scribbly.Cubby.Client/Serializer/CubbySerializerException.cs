using System.Text.Json;

namespace Scribbly.Cubby.Client.Serializer;

/// <summary>
/// An exception raised whenever the type serailized is unknown.
/// </summary>
/// <remarks>Ensure all types have been registered in the CubbyClientOptions builder.</remarks>
/// <example>
/// In Program.cs ensure types have been added to your serializer. 
/// <code>
/// builder.Services
///     .AddCubbyClient(ops =>
///     {
///         // ** System.Text.Json **
///         ops.AddSystemTextSerializer(ops =>
///         {
///             // Add types here.
///             ops.TypeInfoResolverChain.Insert(0, ItemJsonContext.Default);
///         });
///
///         // ** MessagePack **
///         ops.AddMessagePackSerializer(Witness.GeneratedTypeShapeProvider);
///     })
/// </code>
/// </example>
/// <remarks>See</remarks>
/// <typeparam name="TType"></typeparam>
public sealed class CubbySerializerException<TType> : JsonException
{
    /// <summary>
    /// Creates a new exception for the type.
    /// </summary>
    public CubbySerializerException()
        :base($"Unknown JSON Type {typeof(TType)}, Please ensure the type has been registered with the JSON Options.")
    {
        
    }
}