using Scribbly.Cubby.Client;

namespace Scribbly.Cubby;

/// <summary>
/// Specifically targets the HTTP transport.
/// This is required to resolve the explicate HTTP transport.
/// </summary>
internal interface IHttpCubbyStoreTransport : ICubbyStoreTransport;