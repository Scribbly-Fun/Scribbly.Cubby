using Scribbly.Cubby.IntegrationTests.Setup;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports.Http;

[Collection(Collections.HttpCollection)]
public class ExistsEntryTestBase(HttpApplicationFactory application) 
    : Exists_Entry_Client_TestBase<Program>(application);