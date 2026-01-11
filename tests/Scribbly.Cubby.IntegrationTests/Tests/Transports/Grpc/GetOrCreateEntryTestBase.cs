using Scribbly.Cubby.IntegrationTests.Setup;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports.Grpc;


[Collection(Collections.GrpcCollection)]
public class GetOrCreateTestBase(GrpcApplicationFactory application) 
    : GetOrCreate_Entry_Client_TestBase<Program>(application);