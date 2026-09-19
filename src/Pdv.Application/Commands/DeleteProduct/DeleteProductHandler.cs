using MediatR;
using Microsoft.Extensions.Logging;
using Pdv.Domain.Common;
using Pdv.Domain.Interfaces;

namespace Pdv.Application.Commands.DeleteProduct;

public sealed class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IProductRepository _repository;
    private readonly ILogger<DeleteProductHandler> _logger;

    public DeleteProductHandler(IProductRepository repository, ILogger<DeleteProductHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    public async Task<Result> Handle(DeleteProductCommand command, CancellationToken ct)
    {
        var product = await _repository.FirstOrDefaultAsync(command.id, ct);

        if (product is null)
        {
            _logger.LogWarning("Product com id {ProductId} não encontrado.", command.id);
            return Result.Fail("Product não encontrado.");   
        }

        product.MarkAsDeleted();

        var deleted = await _repository.SoftDeleteAsync(product, ct);

        if (!deleted)
        {
            _logger.LogError("Erro ao deletar o produto com id {ProductId}.", command.id);
            return Result.Fail("Erro ao deletar o produto.");
        }

        return Result.Ok();
    }
}
