using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class ActualizarLoteValidatorDto : AbstractValidator<actualizarLoteDto>
    {
        public ActualizarLoteValidatorDto()
        {
            RuleFor(x => x.ProductoId)
            .NotEmpty().WithMessage("El ID del producto no es válido.")
            .When(x => x.ProductoId.HasValue);

            RuleFor(x => x.CodigoLote)
                .NotEmpty().WithMessage("El código de lote no puede estar vacío.")
                .MaximumLength(30).WithMessage("El código de lote no debe superar 30 caracteres.")
                .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("El código de lote contiene caracteres no válidos.")
                .When(x => x.CodigoLote != null);

            RuleFor(x => x.FechaVencimiento)
                .GreaterThan(DateTime.UtcNow.Date)
                .WithMessage("La fecha de vencimiento debe ser posterior a la fecha actual.")
                .When(x => x.FechaVencimiento.HasValue);

            RuleFor(x => x.PrecioCostoUnitario)
                .GreaterThan(0)
                .WithMessage("El precio de costo unitario debe ser mayor a 0.");
        }
    }
}
