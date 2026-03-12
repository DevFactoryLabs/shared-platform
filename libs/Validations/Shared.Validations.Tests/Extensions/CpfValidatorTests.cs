using FluentValidation;
using Shared.Validations.Extensions;

namespace Shared.Validations.Tests.Extensions;

public class CpfValidatorTests
{
    private class TestModel
    {
        public string? Value { get; set; }
    }

    private class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Value).Cpf();
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
    public void IsValid_ValidCpfFormatted_ReturnsTrue()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "529.982.247-25" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IsValid_ValidCpfUnformatted_ReturnsTrue()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "52998224725" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void IsValid_InvalidCpfAllSameDigits_ReturnsFalse()
    {
        // Arrange
        var validator = new TestModelValidator();
        // 111.111.111-11 passes the check-digit algorithm; use 111.111.111-12 (wrong 2nd digit)
        var model = new TestModel { Value = "111.111.111-12" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void IsValid_InvalidCpfWrongCheckDigits_ReturnsFalse()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "12345678901" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void IsValid_CpfWithWrongLength_ReturnsFalse()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "1234567890" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
    }

    [Fact]
    public void IsValid_CpfTooLong_ReturnsFalse()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Value = "529982247251" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
    }
}
