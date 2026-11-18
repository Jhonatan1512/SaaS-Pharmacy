using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class PagoVentaCreateDtoValidator : AbstractValidator<PagoVentaCreateDto>
    {
        public PagoVentaCreateDtoValidator()
        {
            RuleFor(x => x.MedioPago)
                .IsInEnum()
                .WithMessage("El medio de pago no es válido.");

            RuleFor(x => x.Monto)
                .GreaterThan(0)
                .WithMessage("El monto del pago debe ser mayor a cero.");

        }
    }
}
