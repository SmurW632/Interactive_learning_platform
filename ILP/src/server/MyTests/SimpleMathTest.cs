using Xunit;

namespace server.Tests;

public class SimpleMathTest
{
    [Fact]
    public void TwoPlusTwoEqualsFour()
    {
        int result = 2 + 2;
        Assert.Equal(4, result);
    }
}
