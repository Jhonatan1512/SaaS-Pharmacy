using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using PharmacySystem.Application.DTOs;

namespace PharmacySystem.Application.Validatos
{
    public class SedeValidatorDto : AbstractValidator<SedeDto>
    {
        public SedeValidatorDto()
        {
            RuleFor(s => s.Nombre)
                .NotEmpty().WithMessage("El nombre es obligatorio");
            RuleFor(s => s.Direccion)
                .NotEmpty().WithMessage("La dirección es obligatoria");
            RuleFor(s => s.Departamento)
                .NotEmpty().WithMessage("El departamento es obligatorio");
            RuleFor(s => s.Provincia)
                .NotEmpty().WithMessage("La provincia es obligatorio");
            RuleFor(s => s.Distrito)
                .NotEmpty().WithMessage("El distrito es obligatorio");
            RuleFor(s => s.Ubigeo)
                .NotEmpty().WithMessage("El ubigéo es obligatorio")
                .Matches(@"^\d{6}").WithMessage("El ubigéo debe tener exactamente 6 dígitos numéricos");
        }
    }
}
