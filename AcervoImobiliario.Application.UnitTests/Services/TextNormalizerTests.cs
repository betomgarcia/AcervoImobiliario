using AcervoImobiliario.Application.Services;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Services;

public class TextNormalizerTests
{
    private readonly TextNormalizer _sut = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Normalize_ComValorNuloOuVazio_DeveRetornarStringVazia(string? value)
    {
        // Act
        var result = _sut.Normalize(value);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void Normalize_ComAcentosEspacosEMaiusculas_DeveNormalizarTexto()
    {
        // Arrange
        const string value = "  São   Paulo  ";

        // Act
        var result = _sut.Normalize(value);

        // Assert
        result.Should().Be("sao paulo");
    }

    [Fact]
    public void Normalize_ComEspacosDuplicados_DeveRemoverEspacosExtras()
    {
        // Arrange
        const string value = "belo    horizonte";

        // Act
        var result = _sut.Normalize(value);

        // Assert
        result.Should().Be("belo horizonte");
    }
}
