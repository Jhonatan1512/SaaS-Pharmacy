using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class ConfirmarRecepcionInventarioValidatorDto : AbstractValidator<ConfirmarRecepcionInventarioDto>
    {
        public ConfirmarRecepcionInventarioValidatorDto()
        {
            RuleFor(x => x.Observacion)
                .NotEmpty().WithMessage("La observación no puede estar vacia.");
            RuleFor(x => x.CantidadDaniada)
                .GreaterThanOrEqualTo(0).WithMessage("La cantidad de productos dañados no puede ser menor a 0");
        }
    }
}
