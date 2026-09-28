using FluentValidation;
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
    private readonly IValidator<CreateSeelingPriceCommand> _createValidator;

    public SeelingPriceController(IMediator mediator, ILogger<SeelingPriceController> logger, IValidator<CreateSeelingPriceCommand> createValidator)
    {
        _mediator = mediator;
        _logger = logger;
        _createValidator = createValidator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSeelingPrice([FromBody] CreateSeelingPriceRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Recebida a requisição para criar um novo seelingPrice: {ProductId}", request.ProductId);
        var command = new CreateSeelingPriceCommand(request.ProductId, request.Price);

        var validationResult = await _createValidator.ValidateAsync (command, ct);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação de dados falhou para o ProductId: {ProductId}, Erros: {Erros}", request.ProductId, validationResult.Errors);
            return BadRequest(new
            {
                status = 400,
                message = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

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
        return CreatedAtAction(nameof(ProductController.GetProductById), "Product",
          new { id = request.ProductId }, result.Value);
    }
}
