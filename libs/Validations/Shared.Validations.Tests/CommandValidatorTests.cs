using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Shared.Validations.Extensions;

namespace Shared.Validations.Tests;

public class CommandValidatorTests
{
    private class TestCommand
    {
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
    }

    private class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Age).GreaterThan(0).WithMessage("Age must be greater than 0");
        }
    }

    [Fact]
    public void Validate_ValidCommand_ReturnsSuccess()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var commandValidator = new CommandValidator<TestCommand>(validator);
        var command = new TestCommand { Name = "Test", Age = 25 };

        // Act
        var result = commandValidator.Validate(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_InvalidCommand_ReturnsErrors()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var commandValidator = new CommandValidator<TestCommand>(validator);
        var command = new TestCommand { Name = "", Age = -1 };

        // Act
        var result = commandValidator.Validate(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(2, result.Errors.Length);
        Assert.Contains(
            result.Errors,
            e => e.Code == "Name" && e.Description == "Name is required"
        );
        Assert.Contains(
            result.Errors,
            e => e.Code == "Age" && e.Description == "Age must be greater than 0"
        );
    }

    [Fact]
    public void Validate_PartiallyValidCommand_ReturnsSpecificErrors()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var commandValidator = new CommandValidator<TestCommand>(validator);
        var command = new TestCommand { Name = "Test", Age = -1 };

        // Act
        var result = commandValidator.Validate(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Single(result.Errors);
        Assert.Equal("Age", result.Errors[0].Code);
        Assert.Equal("Age must be greater than 0", result.Errors[0].Description);
    }

    [Fact]
    public void Validate_NullCommand_ThrowsInvalidOperationException()
    {
        // Arrange
        var validator = new TestCommandValidator();
        var commandValidator = new CommandValidator<TestCommand>(validator);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => commandValidator.Validate(null!));
    }

    [Fact]
    public void Validate_CommandWithMultipleErrorsOnSameProperty_ReturnsAllErrors()
    {
        // Arrange
        var validator = new TestCommandWithMultipleRulesValidator();
        var commandValidator = new CommandValidator<TestCommand>(validator);
        var command = new TestCommand { Name = "", Age = 25 };
        var collection = new[] { "Name is required", "Name must be at least 2 characters" };

        // Act
        var result = commandValidator.Validate(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.True(result.Errors.Length >= 2);
        Assert.All(
            result.Errors.Where(e => e.Code == "Name"),
            error => Assert.Contains(error.Description, collection)
        );
    }

    [Fact]
    public void Constructor_WithNullValidator_DoesNotThrowImmediately()
    {
        // Arrange & Act
        var commandValidator = new CommandValidator<TestCommand>(null!);

        // Assert
        Assert.NotNull(commandValidator);

        // But should throw when trying to validate
        var command = new TestCommand { Name = "Test", Age = 25 };
        Assert.Throws<NullReferenceException>(() => commandValidator.Validate(command));
    }

    private class TestCommandWithMultipleRulesValidator : AbstractValidator<TestCommand>
    {
        public TestCommandWithMultipleRulesValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required")
                .MinimumLength(2)
                .WithMessage("Name must be at least 2 characters");
            RuleFor(x => x.Age).GreaterThan(0).WithMessage("Age must be greater than 0");
        }
    }
}

public class ValidatorExtensionsTests
{
    private class TestModel
    {
        public string? Cpf { get; set; }
        public string? Cnpj { get; set; }
    }

    private class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(x => x.Cpf).Cpf();
            RuleFor(x => x.Cnpj).Cnpj();
        }
    }

    [Fact]
    public void Cpf_ValidCpf_PassesValidation()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Cpf = "529.982.247-25", Cnpj = null };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Cpf_InvalidCpf_FailsValidation()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Cpf = "111.111.111-12", Cnpj = null };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Cpf");
    }

    [Fact]
    public void Cnpj_ValidCnpj_PassesValidation()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Cpf = null, Cnpj = "11.222.333/0001-81" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Cnpj_InvalidCnpj_FailsValidation()
    {
        // Arrange
        var validator = new TestModelValidator();
        var model = new TestModel { Cpf = null, Cnpj = "11.111.111/1111-11" };

        // Act
        var result = validator.Validate(model);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Cnpj");
    }
}

public class ValidationExtensionsTests
{
    private class SampleCommand { }

    [Fact]
    public void AddValidations_RegistersICommandValidatorAsTransient()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        services.AddValidations();

        // Assert
        var descriptor = Assert.Single(
            services,
            sd => sd.ServiceType == typeof(ICommandValidator<>)
        );
        Assert.Equal(ServiceLifetime.Transient, descriptor.Lifetime);
        Assert.Equal(typeof(CommandValidator<>), descriptor.ImplementationType);
    }

    [Fact]
    public void AddValidations_CanResolveICommandValidatorWithConcreteType()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddTransient<IValidator<SampleCommand>, InlineValidator<SampleCommand>>();
        services.AddValidations();
        var provider = services.BuildServiceProvider();

        // Act
        var validator = provider.GetService<ICommandValidator<SampleCommand>>();

        // Assert
        Assert.NotNull(validator);
    }
}
