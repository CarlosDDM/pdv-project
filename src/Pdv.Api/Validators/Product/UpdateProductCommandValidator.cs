using FluentValidation;
using Pdv.Application.Commands.UpdateProduct;

namespace Pdv.Api.Validators.Product
{
    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do produto é obrigatório.")
                .MaximumLength(100).WithMessage("O nome do produto não pode ter mais de 100 caracteres.");

            RuleFor(x => x.Type)
                .IsInEnum().WithMessage("O tipo do produto é inválido.");

            RuleFor(x => x.Brand)
                .MaximumLength(50).WithMessage("A marca do produto não pode ter mais de 50 caracteres.");

            RuleFor(x => x.Barcode)
                .Length(8, 20).WithMessage("O código de barras do produto deve ter entre 8 e 20 caracteres.")
                .Matches(@"^[0-9]+\z").WithMessage("O código de barras deve conter apenas números.");
        }
    }
}
