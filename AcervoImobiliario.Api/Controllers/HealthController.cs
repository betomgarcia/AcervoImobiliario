using AcervoImobiliario.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AcervoImobiliario.Api.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    private readonly MongoDbContext _mongoDbContext;

    public HealthController(MongoDbContext mongoDbContext)
    {
        _mongoDbContext = mongoDbContext;
    }

    /// <summary>
    /// Liveness — confirma que a API está em execução.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HealthResponse), StatusCodes.Status200OK)]
    public ActionResult<HealthResponse> GetLiveness()
    {
        return Ok(new HealthResponse(
            Status: "Healthy",
            Service: "Acervo Imobiliário API",
            CheckedAt: DateTime.UtcNow));
    }

    /// <summary>
    /// Readiness — confirma que a API consegue acessar o MongoDB.
    /// </summary>
    [HttpGet("ready")]
    [ProducesResponseType(typeof(HealthReadinessResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(HealthReadinessResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<HealthReadinessResponse>> GetReadiness(
        CancellationToken cancellationToken)
    {
        try
        {
            await _mongoDbContext.Database.RunCommandAsync<BsonDocument>(
                new BsonDocument("ping", 1),
                cancellationToken: cancellationToken);

            return Ok(new HealthReadinessResponse(
                Status: "Healthy",
                Service: "Acervo Imobiliário API",
                Database: "Connected",
                CheckedAt: DateTime.UtcNow));
        }
        catch (Exception)
        {
            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                new HealthReadinessResponse(
                    Status: "Unhealthy",
                    Service: "Acervo Imobiliário API",
                    Database: "Disconnected",
                    CheckedAt: DateTime.UtcNow));
        }
    }
}

public sealed record HealthResponse(
    string Status,
    string Service,
    DateTime CheckedAt);

public sealed record HealthReadinessResponse(
    string Status,
    string Service,
    string Database,
    DateTime CheckedAt);
