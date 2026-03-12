using FluentValidation;
using Shared.Validations.Extensions;

namespace Shared.Validations.Tests.Extensions;

public class CnpjValidatorTests
{
    private class TestModel
    {
        public string? Value { get; set; }
    }

    private class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Value).Cnpj();
        }
    }

    [Fact]
    public void IsValid_NullValue_ReturnsTrue()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = null };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IsValid_EmptyValue_ReturnsTrue()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = string.Empty };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IsValid_WhitespaceValue_ReturnsTrue()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "   " };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IsValid_ValidCnpjFormatted_ReturnsTrue()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "11.222.333/0001-81" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IsValid_ValidCnpjUnformatted_ReturnsTrue()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "11222333000181" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IsValid_InvalidCnpjAllSameDigits_ReturnsFalse()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "11.111.111/1111-11" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void IsValid_InvalidCnpjWrongCheckDigits_ReturnsFalse()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "11222333000199" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void IsValid_CnpjWithWrongLength_ReturnsFalse()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "1122233300018" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void IsValid_CnpjTooLong_ReturnsFalse()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "112223330001811" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
    }
}
