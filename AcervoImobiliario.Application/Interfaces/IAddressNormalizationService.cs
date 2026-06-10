namespace AcervoImobiliario.Application.Interfaces;

public interface IAddressNormalizationService
{
    string NormalizeComplement(string? complement);
}
