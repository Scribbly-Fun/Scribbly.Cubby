using Scribbly.Cubby.IntegrationTests.Setup;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports.Grpc;

[Collection(Collections.GrpcCollection)]
public class RefreshEntryTestBase(GrpcApplicationFactory application) 
    : Refresh_Entry_Client_TestBase<Program>(application);