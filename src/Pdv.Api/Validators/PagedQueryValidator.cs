using FluentValidation;
using Pdv.Application.DTOs.Requests;

namespace Pdv.Api.Validators;

public class PagedQueryValidator<T> : AbstractValidator<T> where T : PagedRequest
{

    public PagedQueryValidator()
    {
        RuleFor(x => x.Page)
                    .GreaterThanOrEqualTo(1).WithMessage("A página deve ser igual a 1 ou maior.");
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("O tamanho da página deve ser de 1 ou maior.");
    }
}
