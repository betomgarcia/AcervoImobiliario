using AcervoImobiliario.Domain.Exceptions;

namespace AcervoImobiliario.Domain.Entities;

public sealed class City
{
    public string Id { get; }
    public string Name { get; private set; }
    public string NameNormalized { get; private set; }
    public string State { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; }
    public DateTime? UpdatedAt { get; private set; }

    private City(
        string id,
        string name,
        string nameNormalized,
        string state,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt)
    {
        Id = id;
        Name = name;
        NameNormalized = nameNormalized;
        State = state;
        IsActive = isActive;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static City Restore(
        string id,
        string name,
        string nameNormalized,
        string state,
        bool isActive,
        DateTime createdAt,
        DateTime? updatedAt) =>
        new(id, name, nameNormalized, state, isActive, createdAt, updatedAt);

    public static City Create(string id, string name, string nameNormalized, string state)
    {
        ValidateId(id);
        ValidateName(name);
        ValidateNameNormalized(nameNormalized);
        ValidateState(state);

        return new City(
            id.Trim(),
            name.Trim(),
            nameNormalized,
            state.Trim().ToUpperInvariant(),
            isActive: true,
            createdAt: DateTime.UtcNow,
            updatedAt: null);
    }

    public void Update(string name, string nameNormalized, string state, bool isActive)
    {
        ValidateName(name);
        ValidateNameNormalized(nameNormalized);
        ValidateState(state);

        Name = name.Trim();
        NameNormalized = nameNormalized;
        State = state.Trim().ToUpperInvariant();
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

    private static void ValidateId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new DomainException("O identificador da cidade é obrigatório.");
        }
    }

    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainException("O nome da cidade é obrigatório.");
        }
    }

    private static void ValidateNameNormalized(string nameNormalized)
    {
        if (string.IsNullOrWhiteSpace(nameNormalized))
        {
            throw new DomainException("O nome normalizado da cidade é obrigatório.");
        }
    }

    private static void ValidateState(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
        {
            throw new DomainException("O estado da cidade é obrigatório.");
        }
    }
}
