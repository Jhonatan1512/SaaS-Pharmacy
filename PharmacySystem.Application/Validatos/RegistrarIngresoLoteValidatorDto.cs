using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class RegistrarIngresoLoteValidatorDto : AbstractValidator<RegistrarIngresoLoteDto>
    {
        public RegistrarIngresoLoteValidatorDto()
        {
            RuleFor(x => x.ProductoId)
            .NotEmpty().WithMessage("El identificador del producto es obligatorio.");

            RuleFor(x => x.CodigoLote)
                .NotEmpty().WithMessage("El código de lote es obligatorio.")
                .MaximumLength(50).WithMessage("El código de lote no puede exceder 50 caracteres.");

            RuleFor(x => x.FechaVencimiento)
                .GreaterThan(DateTime.UtcNow.Date)
                .WithMessage("La fecha de vencimiento del lote debe ser posterior a la fecha actual.");

            RuleFor(x => x.PrecioCostoUnitario)
                .GreaterThan(0).WithMessage("El precio de costo unitario debe ser mayor a 0.");

            RuleFor(x => x.CantidadUnidades)
                .GreaterThan(0).WithMessage("La cantidad de unidades que ingresan debe ser mayor a 0.");

            RuleFor(x => x.StockMinimoAlmacenCentral)
                .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");

            RuleFor(x => x.ReferenciaDocumento)
                .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.ReferenciaDocumento));
        }
    }
}
