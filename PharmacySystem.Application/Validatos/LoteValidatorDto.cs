using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class LoteValidatorDto : AbstractValidator<LoteDto>
    {
        public LoteValidatorDto()
        {
            RuleFor(x => x.ProductoId)
            .NotEmpty().WithMessage("El producto es obligatorio.");

            RuleFor(x => x.CodigoLote)
                .NotEmpty().WithMessage("El código de lote es obligatorio.")
                .MaximumLength(30).WithMessage("El código de lote no debe superar 30 caracteres.")
                .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("El código de lote contiene caracteres no válidos.");

            RuleFor(x => x.FechaVencimiento)
                .GreaterThan(DateTime.UtcNow.Date)
                .WithMessage("La fecha de vencimiento debe ser posterior a la fecha actual.");

            RuleFor(x => x.PrecioCostoUnitario)
                .GreaterThan(0).WithMessage("El precio de costo unitario debe ser mayor a 0.");
        }
    }
}
