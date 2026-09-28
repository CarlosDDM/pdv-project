using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pdv.Application.Commands.CreateSeelingPrice;
using Pdv.Application.DTOs.Requests;

namespace Pdv.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SeelingPriceController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<SeelingPriceController> _logger;

    public SeelingPriceController(IMediator mediator, ILogger<SeelingPriceController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSeelingPrice([FromBody] CreateSeelingPriceRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Recebida a requisição para criar um novo seelingPrice: {ProductId}", request.ProductId);
        var command = new CreateSeelingPriceCommand(request.ProductId, request.Price);

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {
            _logger.LogWarning("Não foi possivel fazer a criação do SeelingPrice: {Error}", result.Error);
            return BadRequest(new
            {
                status = 400,
                message = result.Error
            });
        }

        _logger.LogInformation("SeelingPrice criado com sucesso. Id: {Id}", result.Value!.Id);
        return CreatedAtAction(nameof(GetSeelingPriceById), new { id = result.Value!.Id }, result.Value);

    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetSeelingPriceById()
    {
        return BadRequest();
    }
}
