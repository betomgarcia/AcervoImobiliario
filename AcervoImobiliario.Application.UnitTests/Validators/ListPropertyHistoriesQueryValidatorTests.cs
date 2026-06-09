using AcervoImobiliario.Application.Common;
using AcervoImobiliario.Application.Enums;
using AcervoImobiliario.Application.UnitTests.Common;
using AcervoImobiliario.Application.Validators;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Validators;

public class ListPropertyHistoriesQueryValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("desc")]
    [InlineData("DESC")]
    public void ParseSortDirection_SemValorOuDesc_DeveRetornarDesc(string? sortDirection)
    {
        // Act
        var result = ListPropertyHistoriesQueryValidator.ParseSortDirection(sortDirection);

        // Assert
        result.ShouldBeSuccess().Should().Be(HistorySortDirection.Desc);
    }

    [Theory]
    [InlineData("asc")]
    [InlineData("ASC")]
    public void ParseSortDirection_ComAsc_DeveRetornarAsc(string sortDirection)
    {
        // Act
        var result = ListPropertyHistoriesQueryValidator.ParseSortDirection(sortDirection);

        // Assert
        result.ShouldBeSuccess().Should().Be(HistorySortDirection.Asc);
    }

    [Fact]
    public void ParseSortDirection_ComValorInvalido_DeveLancarValidationException()
    {
        // Act
        var result = ListPropertyHistoriesQueryValidator.ParseSortDirection("invalid");

        // Assert
        result.ShouldBeFailure(ErrorKind.Validation, "O parâmetro sortDirection deve ser 'asc' ou 'desc'.");
    }
}
