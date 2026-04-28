namespace server.Tests;

public class BasicTests
{
    [Fact]
    public void AddNumbers_ReturnsCorrectSum()
    {
        // Arrange
        int a = 2;
        int b = 3;

        // Act
        int result = a + b;

        // Assert
        Assert.Equal(5, result);
    }

    [Fact]
    public void StringToUpper_ReturnsUppercase()
    {
        // Arrange
        string input = "hello";

        // Act
        string result = input.ToUpper();

        // Assert
        Assert.Equal("HELLO", result);
    }

    [Fact]
    public void IsEven_ReturnsTrueForEvenNumber()
    {
        // Arrange
        int number = 4;

        // Act
        bool isEven = number % 2 == 0;

        // Assert
        Assert.True(isEven);
    }
}
