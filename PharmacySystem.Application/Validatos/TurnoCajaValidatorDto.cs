using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class TurnoCajaValidatorDto : AbstractValidator<TurnoCajaDto>
    {
        public TurnoCajaValidatorDto()
        {
            RuleFor(t => t.SaldoInicialEfectivo)
                .GreaterThanOrEqualTo(0).WithMessage("El saldo inicial no puede ser negativo");
        }
    }
}
