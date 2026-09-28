using MediatR;
using Microsoft.Extensions.Logging;
using Pdv.Application.DTOs.Responses;
using Pdv.Domain.Common;
using Pdv.Domain.Entities;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Commands.CreateSeelingPrice;

public sealed class CreateSeelingPriceHandler : IRequestHandler<CreateSeelingPriceCommand, Result<SeelingPriceResponse>>
{
    private readonly ISeelingPriceRepository _repository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CreateSeelingPriceHandler> _logger;

    public CreateSeelingPriceHandler(ISeelingPriceRepository repository,IProductRepository productResitory, ILogger<CreateSeelingPriceHandler> logger)
    {
        _repository = repository;
        _productRepository = productResitory;
        _logger = logger;
    }

    public async Task<Result<SeelingPriceResponse>> Handle(CreateSeelingPriceCommand command, CancellationToken ct)
    {
        _logger.LogInformation("Handling CreateSeelingPriceCommand para o Product: {ProductId}", command.ProductId);

        var product = await _productRepository.FirstOrDefaultAsync(command.ProductId, ct);

        if (product is null) 
        {
            _logger.LogWarning("O Product de Id: {ProductId}, não encontrado.", command.ProductId);
            return Result<SeelingPriceResponse>.Fail("O Product não existe ou não encontrado.");
        }

        var result = SeelingPrice.Create(command.ProductId, command.Price);

        if (result.IsFailure)
        {
            _logger.LogWarning("Não foi possivel criar SeelingPrice: ${Error}", result.Error);
            return Result<SeelingPriceResponse>.Fail(result.Error);
        }

        var seelingPrice = result.Value!;

        await _repository.AddAsync(seelingPrice, ct) ;
        _logger.LogInformation("SeelingPrice criado com sucesso. Id: {Id}", seelingPrice.Id);

        return Result<SeelingPriceResponse>.Ok(new SeelingPriceResponse(
            Id: seelingPrice.Id,
            Price: seelingPrice.Price,
            CreatedAt: seelingPrice.CreatedAt
            ));
    }
}
