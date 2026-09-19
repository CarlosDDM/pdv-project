using MediatR;
using Microsoft.Extensions.Logging;
using Pdv.Application.DTOs.Responses;
using Pdv.Application.Interfaces;
using Pdv.Domain.Common;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Commands.UpdateProductImage;

public sealed class UpdateProductImageHandler : IRequestHandler<UpdateProductImageCommand, Result<ProductUpdateImageResponse>>
{
    private readonly IStorageService _storage;
    private readonly IProductRepository _repository;
    private readonly ILogger<UpdateProductImageHandler> _logger;
    private const string _keyPrefix = "products";

    public UpdateProductImageHandler(IStorageService storage, IProductRepository repository, ILogger<UpdateProductImageHandler> logger)
    {
        _storage = storage;
        _repository = repository;
        _logger = logger;
    }
    public async Task<Result<ProductUpdateImageResponse>> Handle(UpdateProductImageCommand command, CancellationToken ct)
    {
        _logger.LogInformation("Handling UpdateProductImageCommand para o Product Id: {ProductId}", command.Id);

        var product = await _repository.FirstOrDefaultAsync(command.Id, ct);

        if (product is null)
        {
            _logger.LogWarning("Produto com ID {ProductId} não encontrado.", command.Id);
            return Result<ProductUpdateImageResponse>.Fail("Produto não encontrado.");
        }

        var oldKey = product.ImageUrl;

        string key;

        try
        {
           key = await _storage.UploadAsync(command.File, command.ContentType, _keyPrefix, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao fazer upload da imagem para o produto com ID {ProductId}.", command.Id);
            return Result<ProductUpdateImageResponse>.Fail("Falha ao fazer upload da imagem.");
        }

        var setImageResult = product.SetImage(key);
        if (setImageResult.IsFailure)
        {
            _logger.LogWarning("Falha ao definir imagem do produto {ProductId}: {Error}", command.Id, setImageResult.Error);
            return Result<ProductUpdateImageResponse>.Fail(setImageResult.Error);
        }

        await _repository.UpdateAsync(product, ct);

        try
        {
            if (oldKey is not null)
            {
                await _storage.DeleteAsync(oldKey, ct);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Falha ao apagar a imagem existente para o produto com ID {ProductId}.", command.Id);
        }

        return Result<ProductUpdateImageResponse>.Ok(new ProductUpdateImageResponse(_storage.GetPublicUrl(key)));
    }

}
