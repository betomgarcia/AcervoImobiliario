using AcervoImobiliario.Domain.Enums;
using AcervoImobiliario.Domain.Exceptions;

namespace AcervoImobiliario.Domain.Entities;

public sealed class Property
{
    public string Id { get; }
    public string CityId { get; private set; }
    public string CityNameSnapshot { get; private set; }
    public string Neighborhood { get; private set; }
    public string NeighborhoodNormalized { get; private set; }
    public string Street { get; private set; }
    public string StreetNormalized { get; private set; }
    public string Number { get; private set; }
    public ComplementType ComplementType { get; private set; }
    public string? ComplementValue { get; private set; }
    public string? ComplementValueNormalized { get; private set; }
    public string? CadastralIndex { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    private Property(
        string id,
        string cityId,
        string cityNameSnapshot,
        string neighborhood,
        string neighborhoodNormalized,
        string street,
        string streetNormalized,
        string number,
        ComplementType complementType,
        string? complementValue,
        string? complementValueNormalized,
        string? cadastralIndex,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        CityId = cityId;
        CityNameSnapshot = cityNameSnapshot;
        Neighborhood = neighborhood;
        NeighborhoodNormalized = neighborhoodNormalized;
        Street = street;
        StreetNormalized = streetNormalized;
        Number = number;
        ComplementType = complementType;
        ComplementValue = complementValue;
        ComplementValueNormalized = complementValueNormalized;
        CadastralIndex = cadastralIndex;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Property Restore(
        string id,
        string cityId,
        string cityNameSnapshot,
        string neighborhood,
        string neighborhoodNormalized,
        string street,
        string streetNormalized,
        string number,
        ComplementType complementType,
        string? complementValue,
        string? complementValueNormalized,
        string? cadastralIndex,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt) =>
        new(
            id,
            cityId,
            cityNameSnapshot,
            neighborhood,
            neighborhoodNormalized,
            street,
            streetNormalized,
            number,
            complementType,
            complementValue,
            complementValueNormalized,
            cadastralIndex,
            isActive,
            createdAt,
            updatedAt);

    public static Property Create(
        string id,
        string cityId,
        string cityNameSnapshot,
        string neighborhood,
        string neighborhoodNormalized,
        string street,
        string streetNormalized,
        string number,
        ComplementType complementType,
        string? complementValue = null,
        string? complementValueNormalized = null,
        string? cadastralIndex = null)
    {
        ValidateId(id);
        ValidateCityId(cityId);
        ValidateCityNameSnapshot(cityNameSnapshot);
        ValidateNeighborhood(neighborhood);
        ValidateNeighborhoodNormalized(neighborhoodNormalized);
        ValidateStreet(street);
        ValidateStreetNormalized(streetNormalized);
        ValidateNumber(number);
        ValidateComplement(complementType, complementValue, complementValueNormalized);

        return new Property(
            id.Trim(),
            cityId.Trim(),
            cityNameSnapshot.Trim(),
            neighborhood.Trim(),
            neighborhoodNormalized,
            street.Trim(),
            streetNormalized,
            number.Trim(),
            complementType,
            TrimComplementValue(complementType, complementValue),
            complementValueNormalized,
            cadastralIndex?.Trim(),
            isActive: true,
            createdAt: DateTime.UtcNow,
            updatedAt: null);
    }

    public void UpdateAddress(
        string cityId,
        string cityNameSnapshot,
        string neighborhood,
        string neighborhoodNormalized,
        string street,
        string streetNormalized,
        string number,
        ComplementType complementType,
        string? complementValue,
        string? complementValueNormalized,
        string? cadastralIndex,
        bool isActive)
    {
        ValidateCityId(cityId);
        ValidateCityNameSnapshot(cityNameSnapshot);
        ValidateNeighborhood(neighborhood);
        ValidateNeighborhoodNormalized(neighborhoodNormalized);
        ValidateStreet(street);
        ValidateStreetNormalized(streetNormalized);
        ValidateNumber(number);
        ValidateComplement(complementType, complementValue, complementValueNormalized);

        CityId = cityId.Trim();
        CityNameSnapshot = cityNameSnapshot.Trim();
        Neighborhood = neighborhood.Trim();
        NeighborhoodNormalized = neighborhoodNormalized;
        Street = street.Trim();
        StreetNormalized = streetNormalized;
        Number = number.Trim();
        ComplementType = complementType;
        ComplementValue = TrimComplementValue(complementType, complementValue);
        ComplementValueNormalized = complementValueNormalized;
        CadastralIndex = cadastralIndex?.Trim();
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive)
        {
            return;
        }

        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        if (IsActive)
        {
            return;
        }

        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GenerateAddressKey() =>
        string.Join(
            "|",
            CityId,
            NeighborhoodNormalized,
            StreetNormalized,
            Number,
            ComplementType.ToString(),
            ComplementValueNormalized ?? string.Empty);

    private static bool RequiresComplementValue(ComplementType complementType) =>
        complementType is ComplementType.Apartment or ComplementType.Room or ComplementType.Store;

    private static string? TrimComplementValue(ComplementType complementType, string? complementValue)
    {
        if (complementType == ComplementType.None)
        {
            return null;
        }

        return complementValue?.Trim();
    }

    private static void ValidateId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new DomainException("O identificador do imóvel é obrigatório.");
        }
    }

    private static void ValidateCityId(string cityId)
    {
        if (string.IsNullOrWhiteSpace(cityId))
        {
            throw new DomainException("A cidade do imóvel é obrigatória.");
        }
    }

    private static void ValidateCityNameSnapshot(string cityNameSnapshot)
    {
        if (string.IsNullOrWhiteSpace(cityNameSnapshot))
        {
            throw new DomainException("O snapshot do nome da cidade é obrigatório.");
        }
    }

    private static void ValidateNeighborhood(string neighborhood)
    {
        if (string.IsNullOrWhiteSpace(neighborhood))
        {
            throw new DomainException("O bairro do imóvel é obrigatório.");
        }
    }

    private static void ValidateNeighborhoodNormalized(string neighborhoodNormalized)
    {
        if (string.IsNullOrWhiteSpace(neighborhoodNormalized))
        {
            throw new DomainException("O bairro normalizado do imóvel é obrigatório.");
        }
    }

    private static void ValidateStreet(string street)
    {
        if (string.IsNullOrWhiteSpace(street))
        {
            throw new DomainException("A rua do imóvel é obrigatória.");
        }
    }

    private static void ValidateStreetNormalized(string streetNormalized)
    {
        if (string.IsNullOrWhiteSpace(streetNormalized))
        {
            throw new DomainException("A rua normalizada do imóvel é obrigatória.");
        }
    }

    private static void ValidateNumber(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
        {
            throw new DomainException("O número do imóvel é obrigatório.");
        }

        var trimmedNumber = number.Trim();

        if (!trimmedNumber.All(char.IsDigit))
        {
            throw new DomainException("O número do imóvel deve conter somente dígitos.");
        }
    }

    private static void ValidateComplement(
        ComplementType complementType,
        string? complementValue,
        string? complementValueNormalized)
    {
        if (complementType == ComplementType.None)
        {
            if (!string.IsNullOrWhiteSpace(complementValue))
            {
                throw new DomainException("Imóveis sem complemento não devem informar ComplementValue.");
            }

            if (!string.IsNullOrWhiteSpace(complementValueNormalized))
            {
                throw new DomainException("Imóveis sem complemento não devem informar ComplementValueNormalized.");
            }

            return;
        }

        if (RequiresComplementValue(complementType) && string.IsNullOrWhiteSpace(complementValue))
        {
            throw new DomainException(
                $"ComplementValue é obrigatório para o tipo de complemento '{complementType}'.");
        }

        if (RequiresComplementValue(complementType) && string.IsNullOrWhiteSpace(complementValueNormalized))
        {
            throw new DomainException(
                $"ComplementValueNormalized é obrigatório para o tipo de complemento '{complementType}'.");
        }
    }
}
