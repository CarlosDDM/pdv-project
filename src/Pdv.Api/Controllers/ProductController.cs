using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Pdv.Application.Commands.CreateProduct;
using Pdv.Application.Commands.DeleteProduct;
using Pdv.Application.Commands.UpdateProduct;
using Pdv.Application.Commands.UpdateProductImage;
using Pdv.Application.DTOs.Requests;
using Pdv.Application.Queries.FindAllProduct;
using Pdv.Application.Queries.FindProduct;
using Pdv.Application.Queries.FindProductByBarcode;

namespace Pdv.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public sealed class ProductController : Controller
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductController> _logger;
    private readonly IValidator<CreateProductCommand> _validator;
    private readonly IValidator<DeleteProductCommand> _deleteValidator;
    private readonly IValidator<FindProductQuery> _findValidator;
    private readonly IValidator<FindProductByBarcodeQuery> _findByBarcodeValidator;
    private readonly IValidator<UpdateProductCommand> _updateProductValidator;
    private readonly IValidator<FindAllProductQuery> _findAllValidator;
    private readonly IValidator<UpdateProductImageCommand> _updateProductImageValidator;

    public ProductController
        (
            IMediator mediator,
            ILogger<ProductController> logger,
            IValidator<CreateProductCommand> validator,
            IValidator<DeleteProductCommand> deleteValidator,
            IValidator<FindProductQuery> findValidator,
            IValidator<FindProductByBarcodeQuery> findByBarcodeValidator,
            IValidator<UpdateProductCommand> updateProductValidator,
            IValidator<FindAllProductQuery> findAllValidator,
            IValidator<UpdateProductImageCommand> updateProductImageValidator
        )
    {
        _mediator = mediator;
        _logger = logger;
        _validator = validator;
        _deleteValidator = deleteValidator;
        _findValidator = findValidator;
        _findByBarcodeValidator = findByBarcodeValidator;
        _updateProductValidator = updateProductValidator;
        _findAllValidator = findAllValidator;
        _updateProductImageValidator = updateProductImageValidator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Recebida a requisição para criar um novo produto: {ProductName}", request.Name);
        var command = new CreateProductCommand(
            request.Name,
            request.Type,
            request.Brand,
            request.Barcode
        );

        var validationResult = await _validator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação falhou para o produto: {ProductName}. Erros: {Errors}", request.Name, validationResult.Errors);
            return BadRequest(new
            {
                status = 400,
                message = "Falha na validação do produto.",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {
            _logger.LogWarning("Falha ao criar o produto: {Error}", result.Error);
            if (result.Error.Contains("já existe"))
            {
                return Conflict(new
                {
                    status = 409,
                    message = result.Error
                });
            }

            return BadRequest(new 
            {
                status = 400,
                message = result.Error
            });
        }

        _logger.LogInformation("Produto criado com sucesso. Id: {ProductId}", result.Value!.Id);
        return CreatedAtAction(nameof(GetProductById), new { id = result.Value.Id }, result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts([FromQuery] FindAllProductRequest request, CancellationToken ct = default)
    {
        var query = new FindAllProductQuery { Page = request.Page, PageSize = request.PageSize };

        var validationResult = await _findAllValidator.ValidateAsync(query, ct);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação falhou ao buscar todos os produtos. Erros: {Errors}", validationResult.Errors);
            return BadRequest(new
            {
                status = 400,
                message = "Falha na validação dos parâmetros de paginação.",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var result = await _mediator.Send(query, ct);
        return Ok(result);
    }

    [HttpGet("barcode/{barcode}")]
    public async Task<IActionResult> GetProductByBarcode([FromRoute] string barcode, CancellationToken ct = default)
    {
        _logger.LogInformation("Recebida a requisição para buscar o product com Código de Barras: {ProductBarcode}", barcode);
        
        var query = new FindProductByBarcodeQuery(barcode);

        var validationResult = await _findByBarcodeValidator.ValidateAsync(query, ct);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação falhou ao buscar o produto: {Barcode}. Erros: {Errors}", barcode, validationResult.Errors);
            return BadRequest(new
            {
                status = 400,
                message = "Falha na validação do código de barras.",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var result = await _mediator.Send(query, ct);

        if (result.IsFailure)
        {
            return NotFound(new
            {
                status = 404,
                message = result.Error
            });
        }

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetProductById([FromRoute] Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Recebida a requisição para buscar o product com Id: {ProductId}", id);

        var query = new FindProductQuery(id);

        var validationResult = await _findValidator.ValidateAsync(query, ct);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação falhou ao buscar o produto: {Id}. Erros: {Errors}", id, validationResult.Errors);
            return BadRequest(new
            {
                status = 400,
                message = "Falha na validação do Id.",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var result = await _mediator.Send(query, ct);

        if (result.IsFailure)
        {
            return NotFound(new
            {
                status = 404,
                message = result.Error
            });
        }

        return Ok(result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> PutProduct([FromRoute] Guid id, UpdateProductRequest request, CancellationToken ct = default)
    {
        _logger.LogInformation("Recebida a requisição para atualizar o produto com Id: {ProductId}", id);

        var command = new UpdateProductCommand(id, request.Name, request.Type, request.Brand, request.Barcode);

        var validationResult = await _updateProductValidator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação falhou ao atualizar o produto: {Id}. Erros: {Errors}", id, validationResult.Errors);
            return BadRequest(new
            {
                status = 400,
                message = "Falha na validação dos dados.",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {

            _logger.LogWarning("Falha ao atualizar o produto: {Error}", result.Error);
            if (result.Error.Contains("já existe"))
            {
                return Conflict(new
                {
                    status = 409,
                    message = result.Error
                });
            }

            _logger.LogWarning("Falha ao atualizar o produto: {Error}", result.Error);
            return NotFound(new
            {
                status = 404,
                message = result.Error
            });
        }
        
        return Ok(result.Value);

    }

    [HttpPatch("{id:guid}/image")]
    public async Task<IActionResult> PatchProductImage([FromRoute] Guid id, IFormFile file, CancellationToken ct = default)
    {
        _logger.LogInformation("Recebida a requisição para atualizar a imagem do produto com Id: {ProductId}", id);

        var command = new UpdateProductImageCommand(id, file.OpenReadStream(), file.FileName, file.ContentType);

        var validationResult = await _updateProductImageValidator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação falhou ao atualizar a imagem do produto: {Id}. Erros: {Errors}", id, validationResult.Errors);
            return BadRequest(new
            {
                status = 400,
                message = "Falha na validação dos dados.",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {
            _logger.LogWarning("Falha ao atualizar a imagem do produto: {Error}", result.Error);
            return NotFound(new
            {
                status = 404,
                message = result.Error
            });
        }

        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id, CancellationToken ct = default)
    {
        _logger.LogInformation("Recebida a requisição para deletar o produto com Id: {ProductId}", id);

        var command = new DeleteProductCommand(id);

        var validationResult = await _deleteValidator.ValidateAsync(command, ct);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning("Validação falhou ao deletar o produto: {Id}. Erros: {Errors}", id, validationResult.Errors);
            return BadRequest(new
            {
                status = 400,
                message = "Falha na validação do Id.",
                errors = validationResult.Errors.Select(e => e.ErrorMessage)
            });
        }

        var result = await _mediator.Send(command, ct);

        if (result.IsFailure)
        {

            _logger.LogWarning("Falha ao deletar o produto: {Error}", result.Error);
            return NotFound(new
            {
                status = 404,
                message = result.Error
            });
        }

        _logger.LogInformation("Produto {ProductId} deletado com sucesso.", id);
        return NoContent();
    }

}
