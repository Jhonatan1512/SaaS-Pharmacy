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
    public class ActualizarUsuarioValidatorDto : AbstractValidator<ActualizarUsuarioDto>
    {
        public ActualizarUsuarioValidatorDto()
        {
            When(c => c.NumeroDocumento != null, () =>
            {
                RuleFor(c => c.NumeroDocumento)
                    .NotEmpty().WithMessage("El número de documento no puede estar vacío")
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
            });

            RuleFor(u => u.Email)
                .EmailAddress().WithMessage("el formato de email no es valido")
                .NotEmpty().WithMessage("El email no puede estar vacio")
                .When(u => u.Email != null); ;


            RuleFor(u => u.NombreCompletoRazonSocial)
                .NotEmpty().WithMessage("El nombre no puede estar vacio")
                .When(u => u.NombreCompletoRazonSocial != null); ;
        }
    }
}
