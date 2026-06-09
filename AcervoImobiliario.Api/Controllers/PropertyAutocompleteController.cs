using AcervoImobiliario.Api.Extensions;
using AcervoImobiliario.Application.DTOs.Properties;
using AcervoImobiliario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AcervoImobiliario.Api.Controllers;

[ApiController]
[Route("api/properties")]
public sealed class PropertyAutocompleteController : ControllerBase
{
    private readonly IPropertyAutocompleteService _autocompleteService;

    public PropertyAutocompleteController(IPropertyAutocompleteService autocompleteService)
    {
        _autocompleteService = autocompleteService;
    }

    [HttpGet("neighborhoods/search")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<string>>> SearchNeighborhoods(
        [FromQuery] string cityId,
        [FromQuery] string term,
        CancellationToken cancellationToken)
    {
        var query = new SearchNeighborhoodsAutocompleteQuery(cityId, term);
        var result = await _autocompleteService.SearchNeighborhoodsAsync(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("streets/search")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<string>>> SearchStreets(
        [FromQuery] string cityId,
        [FromQuery] string neighborhood,
        [FromQuery] string term,
        CancellationToken cancellationToken)
    {
        var query = new SearchStreetsAutocompleteQuery(cityId, neighborhood, term);
        var result = await _autocompleteService.SearchStreetsAsync(query, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("numbers/search")]
    [ProducesResponseType(typeof(IReadOnlyList<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<string>>> SearchNumbers(
        [FromQuery] string cityId,
        [FromQuery] string neighborhood,
        [FromQuery] string street,
        [FromQuery] string term,
        CancellationToken cancellationToken)
    {
        var query = new SearchNumbersAutocompleteQuery(cityId, neighborhood, street, term);
        var result = await _autocompleteService.SearchNumbersAsync(query, cancellationToken);
        return result.ToActionResult();
    }
}
