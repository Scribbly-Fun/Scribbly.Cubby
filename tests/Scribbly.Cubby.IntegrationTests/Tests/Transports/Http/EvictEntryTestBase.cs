using Scribbly.Cubby.IntegrationTests.Setup;

namespace Scribbly.Cubby.IntegrationTests.Tests.Transports.Http;

[Collection(Collections.HttpCollection)]
public class EvictEntryTestBase(HttpApplicationFactory application) 
    : Evict_Entry_Client_TestBase<Program>(application);