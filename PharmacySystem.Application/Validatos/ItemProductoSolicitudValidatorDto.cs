using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class ItemProductoSolicitudValidatorDto : AbstractValidator<ItemProductoSolicitudDto>
    {
        public ItemProductoSolicitudValidatorDto()
        {
            RuleFor(p => p.ProductoId)
            .NotEmpty()
            .WithMessage("Debe seleccionar un producto válido");

            RuleFor(p => p.CantidadSolicitadaUnidades)
                .GreaterThan(0)
                .WithMessage("La cantidad solicitada debe ser mayor a cero");
        }
    }
}
