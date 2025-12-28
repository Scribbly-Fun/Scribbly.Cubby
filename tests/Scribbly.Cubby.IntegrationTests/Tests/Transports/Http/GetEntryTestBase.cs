using Scribbly.Cubby.IntegrationTests.Setup;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports.Http;


[Collection(Collections.HttpCollection)]
public class GetEntryTestBase(HttpApplicationFactory application) 
    : Get_Entry_Client_TestBase<Program>(application);