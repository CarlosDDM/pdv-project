using Pdv.Domain.Entities;
using Pdv.Domain.Enums;

namespace Pdv.Application.Tests.Factories;

public sealed class ProductFactory
{
    public const string NomeValido = "Arroz soltinho 1Kg";

    public static Product CriarProductValido(
        string name = NomeValido,
        ProductType type = ProductType.Unit,
        string? brand = null,
        string? barcode = null)
    {
        var result = Product.Create(name, type, brand, barcode);

        return result.Value!;
    }
}