namespace Scribbly.Cubby.IntegrationTests.Setup;

public static class Collections
{
    public const string BaseCollection = "Cubby";
    public const string HttpCollection = "Cubby.Http";
    public const string GrpcCollection = "Cubby.Grpc";
}

[CollectionDefinition(Collections.BaseCollection)]
public class CubbyCollection : ICollectionFixture<ApplicationFactory>;

[CollectionDefinition(Collections.HttpCollection)]
public class CubbyHttpCollection : ICollectionFixture<HttpApplicationFactory>;

[CollectionDefinition(Collections.GrpcCollection)]
public class CubbyGrpcCollection : ICollectionFixture<GrpcApplicationFactory>;
