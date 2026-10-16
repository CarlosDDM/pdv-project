namespace Pdv.Application.DTOs.Requests;

public sealed record CreateSeelingPriceRequest(
    int ProductId,
    decimal Price
    );
