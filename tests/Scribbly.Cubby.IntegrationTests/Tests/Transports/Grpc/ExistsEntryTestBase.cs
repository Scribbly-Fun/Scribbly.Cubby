using Scribbly.Cubby.IntegrationTests.Setup;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports.Grpc;

[Collection(Collections.GrpcCollection)]
public class ExistsEntryTestBase(GrpcApplicationFactory application) 
    : Exists_Entry_Client_TestBase<Program>(application);