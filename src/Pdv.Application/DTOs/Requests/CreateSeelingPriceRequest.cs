namespace Pdv.Application.DTOs.Requests;

public sealed record CreateSeelingPriceRequest(
    Guid ProductId,
    decimal Price
    );
