using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class DetalleAprobacionItemValidatorDto : AbstractValidator<DetalleAprobacionItemDto>
    {
        public DetalleAprobacionItemValidatorDto()
        {
            RuleFor(p => p.ProductoId)
                .NotEmpty()
                .WithMessage("Debe seleccionar un producto válido");

            RuleFor(p => p.CantidadAprobadaUnidades)
                .GreaterThanOrEqualTo(0)
                .WithMessage("La cantidad aprobada debe ser mayor a cero");
        }
    }
}
