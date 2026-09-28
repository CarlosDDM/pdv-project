using Pdv.Domain.Entities;

namespace Pdv.Application.Tests.Factories;

public static class SeelingPriceFactory
{
    public static readonly Guid DefaultProductId = Guid.Parse("9AC1A6F1-4A87-4624-B5F5-B74D494E21E0");
    public const decimal DefaultPrice = 25.50m;

    public static SeelingPrice Criar()
        => SeelingPrice.Create(DefaultProductId, DefaultPrice).Value!;

    public static SeelingPrice Criar(Guid productId, decimal price)
        => SeelingPrice.Create(productId, price).Value!;
}