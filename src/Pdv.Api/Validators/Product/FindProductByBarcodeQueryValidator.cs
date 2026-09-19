using FluentValidation;
using Pdv.Application.Queries.FindProductByBarcode;

namespace Pdv.Api.Validators.Product;

public sealed class FindProductByBarcodeQueryValidator : AbstractValidator<FindProductByBarcodeQuery>
{

    public FindProductByBarcodeQueryValidator()
    {
        RuleFor(x => x.Barcode)
            .NotEmpty().WithMessage("O código de barras é obrigatório.")
            .Length(8, 20).WithMessage("O código de barras deve ter entre 8 e 20 caracteres.");
    }
}
