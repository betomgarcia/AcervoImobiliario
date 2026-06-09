using AcervoImobiliario.Api.Extensions;
using AcervoImobiliario.Application.DTOs.PropertyHistories;
using AcervoImobiliario.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AcervoImobiliario.Api.Controllers;

[ApiController]
[Route("api/properties/{propertyId}/histories")]
public sealed class PropertyHistoriesController : ControllerBase
{
    private readonly IPropertyHistoryService _historyService;

    public PropertyHistoriesController(IPropertyHistoryService historyService)
    {
        _historyService = historyService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PropertyHistoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropertyHistoryResponse>> Create(
        string propertyId,
        [FromBody] CreatePropertyHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _historyService.CreateAsync(propertyId, request, cancellationToken);
        return result.ToCreatedResult(history => $"/api/properties/{propertyId}/histories/{history.Id}");
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PropertyHistoryResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PropertyHistoryResponse>>> List(
        string propertyId,
        [FromQuery] string? sortDirection,
        CancellationToken cancellationToken)
    {
        var result = await _historyService.ListByPropertyIdAsync(
            propertyId,
            sortDirection,
            cancellationToken);

        return result.ToActionResult();
    }
}
