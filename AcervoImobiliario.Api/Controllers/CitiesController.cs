using AcervoImobiliario.Api.Extensions;
using AcervoImobiliario.Application.DTOs.Cities;
using AcervoImobiliario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AcervoImobiliario.Api.Controllers;

[ApiController]
[Route("api/cities")]
public sealed class CitiesController : ControllerBase
{
    private readonly ICityService _cityService;

    public CitiesController(ICityService cityService)
    {
        _cityService = cityService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CityResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CityResponse>>> List(
        [FromQuery] string? name,
        [FromQuery] CityStatusFilter status = CityStatusFilter.Active,
        CancellationToken cancellationToken = default)
    {
        var result = await _cityService.ListAsync(new ListCitiesQuery(name, status), cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<CityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<CityResponse>>> Search(
        [FromQuery] string term,
        CancellationToken cancellationToken)
    {
        var result = await _cityService.SearchAsync(term, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CityResponse>> GetById(
        string id,
        CancellationToken cancellationToken)
    {
        var result = await _cityService.GetByIdAsync(id, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost]
    [ProducesResponseType(typeof(CityResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CityResponse>> Create(
        [FromBody] CreateCityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _cityService.CreateAsync(request, cancellationToken);
        return result.ToCreatedResult(city => $"/api/cities/{city.Id}");
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(CityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CityResponse>> Update(
        string id,
        [FromBody] UpdateCityRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _cityService.UpdateAsync(id, request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{id}/activate")]
    [ProducesResponseType(typeof(CityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CityResponse>> Activate(
        string id,
        CancellationToken cancellationToken)
    {
        var result = await _cityService.ActivateAsync(id, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("{id}/deactivate")]
    [ProducesResponseType(typeof(CityResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CityResponse>> Deactivate(
        string id,
        CancellationToken cancellationToken)
    {
        var result = await _cityService.DeactivateAsync(id, cancellationToken);
        return result.ToActionResult();
    }
}
