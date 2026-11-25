using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class ResponderSolicitudReposicionValidatorDto : AbstractValidator<ResponderSolicitudReposicionDto>
    {
        public ResponderSolicitudReposicionValidatorDto()
        {
            RuleFor(s => s.RespuestaAdministrador)
                .NotEmpty()
                .WithMessage("Debe registrar una respuesta del administrador");

            RuleFor(s => s.ProductosAprobados)
                .NotEmpty()
                .When(s => s.Aprobada)
                .WithMessage("Debe registrar al menos un producto aprobado");

            RuleForEach(s => s.ProductosAprobados)
                .SetValidator(new DetalleAprobacionItemValidatorDto())
                .When(s => s.Aprobada);
        }
    }
}
