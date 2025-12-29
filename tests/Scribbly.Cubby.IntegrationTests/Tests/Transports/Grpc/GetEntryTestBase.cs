using Scribbly.Cubby.IntegrationTests.Setup;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports.Grpc;


[Collection(Collections.GrpcCollection)]
public class GetEntryTestBase(GrpcApplicationFactory application) 
    : Get_Entry_Client_TestBase<Program>(application);