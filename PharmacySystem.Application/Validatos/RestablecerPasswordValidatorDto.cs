using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class RestablecerPasswordValidatorDto : AbstractValidator<RestablecerPasswordDto>
    {
        public RestablecerPasswordValidatorDto()
        {
            RuleFor(x => x.NuevaPassword)
            .NotEmpty().WithMessage("La nueva contraseña es obligatoria")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z0-9]).{6,}$")
            .WithMessage("La contraseña debe tener mínimo 6 caracteres, una mayúscula, una minúscula, un número y un carácter especial");

            RuleFor(x => x.ConfirmarPassword)
                .NotEmpty().WithMessage("Debe confirmar la contraseña")
                .Equal(x => x.NuevaPassword)
                .WithMessage("Las contraseñas no coinciden");
        }
    }
}
