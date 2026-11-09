using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class CorregirPreciosValidatorDto : AbstractValidator<CorregirPrecioInventarioSedeDto>
    {
        public CorregirPreciosValidatorDto()
        {
            RuleFor(x => x.PrecioVentaUnidad)
                .GreaterThan(0).WithMessage("El precio de venta unitario debe ser mayor a 0.");

            RuleFor(x => x.PrecioVentaBlister)
                .GreaterThanOrEqualTo(0).WithMessage("El precio por blíster no puede ser negativo.");

            RuleFor(x => x.PrecioVentaCaja)
                .GreaterThanOrEqualTo(0).WithMessage("El precio por caja no puede ser negativo.");
        }
    }
}
