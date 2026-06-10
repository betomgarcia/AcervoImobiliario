using AcervoImobiliario.Api.Extensions;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AcervoImobiliario.Api.Controllers;

[ApiController]
[Route("api/properties")]
public sealed class PropertiesController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertiesController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<PropertyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<PropertyResponse>>> Search(
        [FromQuery] string? cityId,
        [FromQuery] string? neighborhood,
        [FromQuery] string? street,
        [FromQuery] string? number,
        [FromQuery] string? complement,
        [FromQuery] string? cadastralIndex,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default)
    {
        var query = new SearchPropertiesQuery(
            cityId,
            neighborhood,
            street,
            number,
            complement,
            cadastralIndex,
            includeInactive);

        var result = await _propertyService.SearchAsync(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PropertyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropertyResponse>> GetById(string id, CancellationToken cancellationToken)
    {
        var result = await _propertyService.GetByIdAsync(id, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [ProducesResponseType(typeof(PropertyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropertyResponse>> Create(
        [FromBody] CreatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _propertyService.CreateAsync(request, cancellationToken);
        return result.ToCreatedResult(property => $"/api/properties/{property.Id}");
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(PropertyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropertyResponse>> Update(
        string id,
        [FromBody] UpdatePropertyRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _propertyService.UpdateAsync(id, request, cancellationToken);
        return result.ToActionResult();
    }
}
