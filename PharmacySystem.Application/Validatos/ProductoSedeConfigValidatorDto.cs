using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class ProductoSedeConfigValidatorDto : AbstractValidator<ProductoSedeConfigDto>
    {
        public ProductoSedeConfigValidatorDto()
        {
            RuleFor(c => c.SedeId).NotEmpty().WithMessage("La sede es obligatoria");
            RuleFor(c => c.ProductoId).NotEmpty().WithMessage("El producto es obligatorio");
            RuleFor(c => c.StockMinimoUnidades).NotEmpty().GreaterThan(0).WithMessage("El stock minimo debe ser maytor a 0");

            RuleFor(c => c.StockDeseadoUnidades)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("El stock deseado debe ser mayor a 0")
                .Must((c, stockDeseado) => stockDeseado > c.StockMinimoUnidades)
                .WithMessage("El stock deseado debe ser mayor al stock mínimo");

            RuleFor(c => c.DiasAnticipacionVencimiento).NotEmpty().GreaterThan(0).WithMessage("Los días de anticipación deben ser mayor a 0");
        }
    }
}
