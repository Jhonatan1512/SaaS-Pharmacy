using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class ProductoValidatorDto : AbstractValidator<ProductoDto>
    {
        private static readonly string[] TiposIgvValidos = ["10", "20", "30"];
        public ProductoValidatorDto()
        {
            RuleFor(x => x.NombreComercial)
            .NotEmpty().WithMessage("El nombre comercial es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre comercial no debe superar los 150 caracteres.");

            RuleFor(x => x.PrincipioActivo)
                .NotEmpty().WithMessage("El principio activo es obligatorio.")
                .MaximumLength(150).WithMessage("El principio activo no debe superar los 150 caracteres.");

            RuleFor(x => x.Presentacion)
                .NotEmpty().WithMessage("La presentación es obligatoria (Ej: Caja x 100 tabletas).")
                .MaximumLength(100).WithMessage("La presentación no debe superar los 100 caracteres.");

            RuleFor(x => x.RegistroSanitario)
                .NotEmpty().WithMessage("El Registro Sanitario es obligatorio para productos farmacéuticos.")
                .MaximumLength(30).WithMessage("El Registro Sanitario no puede exceder los 30 caracteres.")
                .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("El Registro Sanitario contiene caracteres no válidos.");

            When(x => !string.IsNullOrWhiteSpace(x.CodigoBarras), () =>
            {
                RuleFor(x => x.CodigoBarras)
                    .MinimumLength(8).WithMessage("El código de barras debe tener al menos 8 caracteres.")
                    .MaximumLength(30).WithMessage("El código de barras no puede exceder los 30 caracteres.")
                    .Matches(@"^[a-zA-Z0-9]+$").WithMessage("El código de barras solo debe contener letras y números.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.CodigoProductoSunat), () =>
            {
                RuleFor(x => x.CodigoProductoSunat)
                    .Length(8).WithMessage("El código de producto SUNAT debe tener exactamente 8 dígitos.")
                    .Matches(@"^\d{8}$").WithMessage("El código de producto SUNAT solo debe contener números.");
            });

            RuleFor(x => x.TipoAfectacionIgvDefault)
                .NotEmpty().WithMessage("El tipo de afectación de IGV es obligatorio.")
                .Must(tipo => TiposIgvValidos.Contains(tipo))
                .WithMessage("El tipo de afectación de IGV no es válido (Valores permitidos: 10, 20, 30).");

            RuleFor(x => x.UnidadesPorCaja)
                .GreaterThan(0).WithMessage("La cantidad de unidades por caja debe ser mayor a 0.");

            RuleFor(x => x.UnidadesPorBlister)
                .GreaterThan(0).WithMessage("La cantidad de unidades por blíster debe ser mayor a 0.")
                .LessThanOrEqualTo(x => x.UnidadesPorCaja)
                .WithMessage("Las unidades por blíster no pueden superar las unidades por caja.");

            RuleFor(x => x.PrecioUnidad)
                .GreaterThan(0).WithMessage("El precio unitario debe ser mayor a 0.");

            RuleFor(x => x.PrecioBlister)
                .GreaterThanOrEqualTo(0).WithMessage("El precio del blíster no puede ser negativo.");

            RuleFor(x => x.PrecioCaja)
                .GreaterThanOrEqualTo(0).WithMessage("El precio de la caja no puede ser negativo.");
        }
    }
}
