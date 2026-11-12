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
    public class ClienteActualizarValidatorDto : AbstractValidator<ClienteUpdateDto>
    {
        public ClienteActualizarValidatorDto()
        {
            When(c => c.NumeroDocumento != null, () =>
            {
                RuleFor(c => c.NumeroDocumento)
                    .NotEmpty().WithMessage("El número de documento no puede estar vacío")
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
            });

            RuleFor(u => u.NombresRazonSocial)
                .NotEmpty().WithMessage("El nombre no puede estar vacío")
                .When(u => u.NombresRazonSocial != null);

            RuleFor(c => c.Telefono)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("El teléfono no puede estar vacío")
                .Matches(@"^[0-9]{9}$").WithMessage("El número de teléfono debe contener exactamente 9 dígitos numéricos")
                .When(c => c.Telefono != null);
        }
    }
}
