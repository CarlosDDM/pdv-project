using FluentValidation;
using Pdv.Application.Queries.FindProduct;

namespace Pdv.Api.Validators.Product;

public class FindProductQueryValidator : AbstractValidator<FindProductQuery>
{
    public FindProductQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id do produto é obrigatório.")
            .GreaterThan(0).WithMessage("O Id do produto é obrigatório e não pode ser zerado ou negativo.");
    }
}
