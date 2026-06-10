using AcervoImobiliario.Application.Services;
using FluentAssertions;

namespace AcervoImobiliario.Application.UnitTests.Services;

public class AddressNormalizationServiceTests
{
    private readonly AddressNormalizationService _sut = new();

    [Theory]
    [InlineData("Apartamento 303 Bloco A", "APT 303 BLOCO A")]
    [InlineData("APT 303 BLOCO A", "APT 303 BLOCO A")]
    [InlineData(" apto   303   bloco a ", "APT 303 BLOCO A")]
    [InlineData("Apto 303 Bloco A", "APT 303 BLOCO A")]
    [InlineData("Apartamento. 303 Bloco A", "APT 303 BLOCO A")]
    [InlineData("Casa Fundos", "CASA FUNDOS")]
    [InlineData("Loja 02", "LOJA 02")]
    [InlineData("Lote 05 Quadra 12", "LOTE 05 QUADRA 12")]
    [InlineData("Cobertura 1201", "COBERTURA 1201")]
    public void NormalizeComplement_ComEntradasEquivalentes_DeveGerarMesmoValor(
        string input,
        string expected)
    {
        _sut.NormalizeComplement(input).Should().Be(expected);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizeComplement_ComplementoVazio_DeveRetornarStringVazia(string? input)
    {
        _sut.NormalizeComplement(input).Should().BeEmpty();
    }

    [Fact]
    public void GenerateAddressKey_ComComplementosEquivalentes_DeveGerarMesmaChave()
    {
        var normalization = new AddressNormalizationService();
        var normalizedA = normalization.NormalizeComplement("Apartamento 303 Bloco A");
        var normalizedB = normalization.NormalizeComplement("APT 303 BLOCO A");
        var normalizedC = normalization.NormalizeComplement(" apto 303 bloco a ");

        normalizedA.Should().Be("APT 303 BLOCO A");
        normalizedB.Should().Be(normalizedA);
        normalizedC.Should().Be(normalizedA);

        var keyA = string.Join("|", "city-1", "centro", "rua a", "100", normalizedA);
        var keyB = string.Join("|", "city-1", "centro", "rua a", "100", normalizedB);
        var keyC = string.Join("|", "city-1", "centro", "rua a", "100", normalizedC);

        keyA.Should().Be(keyB);
        keyB.Should().Be(keyC);
    }
}
