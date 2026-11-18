using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class VentasDtoValidator : AbstractValidator<VentasDto>
    {
        public VentasDtoValidator()
        {
            RuleFor(x => x.ClienteId)
                .NotEmpty().When(x => x.ClienteId.HasValue)
                .WithMessage("El ID del cliente no puede estar vacío si es especificado.");

            RuleFor(x => x.TipoComprobante)
                .IsInEnum()
                .WithMessage("El tipo de comprobante no es válido.");

            RuleFor(x => x.Detalles)
                .NotEmpty()
                .WithMessage("La venta debe contener al menos un detalle de producto.");

            RuleForEach(x => x.Detalles)
                .SetValidator(new DetalleVentaCreateDtoValidator());

            RuleFor(x => x.Pagos)
                .NotEmpty()
                .WithMessage("La venta debe registrar al menos un pago.");

            RuleForEach(x => x.Pagos)
                .SetValidator(new PagoVentaCreateDtoValidator());
        }
    }
}
