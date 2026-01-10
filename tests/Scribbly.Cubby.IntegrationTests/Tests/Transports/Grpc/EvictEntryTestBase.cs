using Scribbly.Cubby.IntegrationTests.Setup;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports.Grpc;

[Collection(Collections.GrpcCollection)]
public class EvictEntryTestBase(GrpcApplicationFactory application) 
    : Evict_Entry_Client_TestBase<Program>(application);