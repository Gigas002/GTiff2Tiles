using NUnit.Framework;

namespace GTiff2Tiles.Tests.Tests.Helpers;

[TestFixture]
public sealed class NetVipsHelperTests
{
    [Test]
    public void DisableLogTest() => Assert.DoesNotThrow(Core.Helpers.NetVipsHelper.DisableLog);
}