using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class InvetarioSedeValidatorDto : AbstractValidator<InventarioSedeDto>
    {
        public InvetarioSedeValidatorDto()
        {
            RuleFor(x => x.SedeId)
            .NotEmpty().WithMessage("La sede es obligatoria.");

            RuleFor(x => x.LoteId)
                .NotEmpty().WithMessage("El lote es obligatorio.");

            RuleFor(x => x.StockFisicoUnidades)
                .GreaterThanOrEqualTo(0).WithMessage("El stock físico no puede ser negativo.");

            RuleFor(x => x.PrecioVentaUnidad)
                .GreaterThan(0).WithMessage("El precio de venta unitario debe ser mayor a 0.");

            RuleFor(x => x.PrecioVentaBlister)
                .GreaterThanOrEqualTo(0).WithMessage("El precio por blíster no puede ser negativo.");

            RuleFor(x => x.PrecioVentaCaja)
                .GreaterThanOrEqualTo(0).WithMessage("El precio por caja no puede ser negativo.");

            RuleFor(x => x.Estado)
                .IsInEnum().WithMessage("El estado asignado no es válido.");
        }
    }
}
