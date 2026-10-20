using FluentValidation;
using PharmacySystem.Application.DTOs;
using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class FarmaceuticoValidatorDto : AbstractValidator<UsuarioDto>
    {
        public FarmaceuticoValidatorDto()
        {
            RuleFor(u => u.NombreCompletoRazonSocial)
                .NotEmpty().WithMessage("El nombre es obligatorio");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("El email es obligatorio")
                .EmailAddress().WithMessage("El formato del email no es válido");

            RuleFor(u => u.NumeroDocumento)
                .NotEmpty().WithMessage("El número de documento es obligatorio")
                .Matches(@"^[0-9]+$").WithMessage("El número de documento debe contener solo dígitos numéricos");

            When(u => u.TipoDocumento == TipoDocumento.DNI, () =>
            {
                RuleFor(u => u.NumeroDocumento)
                    .Length(8).WithMessage("El DNI debe tener exactamente 8 dígitos");
            });

            When(u => u.TipoDocumento == TipoDocumento.RUC, () =>
            {
                RuleFor(u => u.NumeroDocumento)
                    .Length(11).WithMessage("El RUC debe tener exactamente 11 dígitos");
            });
        }
    }
}
