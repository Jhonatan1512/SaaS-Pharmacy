using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class ActualizaCierreCajaValidatorDto : AbstractValidator<CorregirConteoTurnoDto>
    {
        public ActualizaCierreCajaValidatorDto()
        {
            RuleFor(x => x)
            .Must(dto => dto.ConteoFisicoEfectivo.HasValue ||
                         dto.ConteoFisicoYape.HasValue ||
                         dto.ConteoFisicoPlin.HasValue ||
                         dto.ConteoFisicoTarjeta.HasValue)
            .WithMessage("Debe enviar al menos un monto de conteo físico para corregir.");

            RuleFor(x => x.ConteoFisicoEfectivo)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Los montos del conteo físico no pueden ser negativos.");

            RuleFor(x => x.ConteoFisicoYape)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Los montos del conteo físico no pueden ser negativos.");

            RuleFor(x => x.ConteoFisicoPlin)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Los montos del conteo físico no pueden ser negativos.");

            RuleFor(x => x.ConteoFisicoTarjeta)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Los montos del conteo físico no pueden ser negativos.");

            RuleFor(x => x.MotivoCorreccion)
                .NotEmpty().WithMessage("El motivo de la corrección es obligatorio.")
                .MaximumLength(300).WithMessage("El motivo no puede exceder los 300 caracteres.");
        }
    }
}
