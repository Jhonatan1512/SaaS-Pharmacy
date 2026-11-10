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
    public class ClienteCreateValidatorDto : AbstractValidator<ClienteDto>
    {
        public ClienteCreateValidatorDto()
        {
            RuleFor(c => c.NumeroDocumento)
                .NotEmpty().WithMessage("El número de documento es obligatorio")
                .Matches(@"^[0-9]+$").WithMessage("El número de documento debe contener solo dígitos numéricos");

            When(u => u.TipoDocumento == TipoDoc.DNI, () =>
            {
                RuleFor(u => u.NumeroDocumento)
                    .Length(8).WithMessage("El DNI debe tener exactamente 8 dígitos");
            });

            When(u => u.TipoDocumento == TipoDoc.RUC, () =>
            {
                RuleFor(u => u.NumeroDocumento)
                    .Length(11).WithMessage("El RUC debe tener exactamente 11 dígitos");
            });

            RuleFor(u => u.NombresRazonSocial)
                .NotEmpty().WithMessage("El nombre es obligatorio");

            RuleFor(c => c.Telefono)
                .Cascade(CascadeMode.Stop)
                .Matches(@"^[0-9]{9}$")
                .When(c => !string.IsNullOrEmpty(c.Telefono))
                .WithMessage("El número de teléfono debe contener exactamente 9 dígitos numéricos");
        }
    }
}
