using FluentValidation;
using Pdv.Application.Commands.UpdateProductImage;

namespace Pdv.Api.Validators.Product;

public sealed class PatchProductImageCommandValidator : AbstractValidator<UpdateProductImageCommand>
{
    private static readonly string[] AllowedMimeTypes =
{
        "image/jpeg",
        "image/png",
        "image/webp",
    };

    public PatchProductImageCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("O Id do produto é obrigatório.");

        RuleFor(x => x.File)
            .NotNull()
            .Must(file => file.CanSeek && file.Length > 0)
                .WithMessage("Arquivo inválido ou vazio");

        RuleFor(x => x.ContentType)
            .NotEmpty()
            .Must(contentType => AllowedMimeTypes.Contains(contentType))
                .WithMessage($"Tipo de imagem não permitido. Tipos aceitos: {string.Join(", ", AllowedMimeTypes)}");
    }
}
