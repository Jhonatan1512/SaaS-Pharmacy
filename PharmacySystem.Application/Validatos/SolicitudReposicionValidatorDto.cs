using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class SolicitudReposicionValidatorDto : AbstractValidator<CrearSolicitudReposicionDto>
    {
        public SolicitudReposicionValidatorDto()
        {
            RuleFor(s => s.MotivoSolicitud)
                .NotEmpty()
                .WithMessage("Debe registrar un motivo de la solicitud");

            RuleFor(s => s.FechaLimiteRequerida)
                .Must(fecha => !fecha.HasValue || fecha.Value.Date > DateTime.Today)
                .WithMessage("La fecha límite requerida debe ser mayor a la fecha actual");

            RuleFor(s => s.Productos)
                .NotEmpty()
                .WithMessage("Debe agregar al menos un producto a la solicitud");

            RuleForEach(s => s.Productos)
           .SetValidator(new ItemProductoSolicitudValidatorDto());
        }
    }
}
