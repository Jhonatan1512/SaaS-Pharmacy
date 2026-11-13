using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class CerrarTurnoCajaValidatorDto : AbstractValidator<CerrarCajaDto>
    {
        public CerrarTurnoCajaValidatorDto()
        {
            RuleFor(t => t.ConteoFisicoEfectivo)
                .GreaterThanOrEqualTo(0).WithMessage("El conteo físico no puede ser negativo");

            RuleFor(t => t.ConteoFisicoYape)
                .GreaterThanOrEqualTo(0).WithMessage("El conteo físico de yape no puede ser negativo");

            RuleFor(t => t.ConteoFisicoPlin)
                .GreaterThanOrEqualTo(0).WithMessage("El conteo físico de plin no puede ser negativo");

            RuleFor(t => t.ConteoFisicoTarjeta)
                .GreaterThanOrEqualTo(0).WithMessage("El conteo físico de la tarjeta no puede ser negativo");
        }
    }
}
