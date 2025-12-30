using Scribbly.Cubby.IntegrationTests.Setup;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports.Grpc;

[Collection(Collections.GrpcCollection)]
public class PutEntryTestBase(GrpcApplicationFactory application)
    : Put_Entry_Client_TestBase<Program>(application);