using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class DetalleVentaCreateDtoValidator : AbstractValidator<DetalleVentaCreateDto>
    {
        public DetalleVentaCreateDtoValidator()
        {
            RuleFor(x => x.InventarioSedeId)
                .NotEmpty()
                .WithMessage("El ID de inventario de sede es obligatorio.");

            RuleFor(x => x.TipoUnidadVenta)
                .IsInEnum()
                .WithMessage("El tipo de unidad de venta no es válido.");

            RuleFor(x => x.CantidadVenta)
                .GreaterThan(0)
                .WithMessage("La cantidad de venta debe ser mayor a cero.");
        }
    }
}
