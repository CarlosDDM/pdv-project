using Pdv.Domain.Entities;

namespace Pdv.Application.Tests.Factories;

public static class SeelingPriceFactory
{
    public static readonly int DefaultProductId = 1;
    public const decimal DefaultPrice = 25.50m;

    public static SeelingPrice Criar()
        => SeelingPrice.Create(DefaultProductId, DefaultPrice).Value!;

    public static SeelingPrice Criar(int productId, decimal price)
        => SeelingPrice.Create(productId, price).Value!;
}