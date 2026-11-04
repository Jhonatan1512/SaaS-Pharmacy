using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class ActualizarproductoValidatorDto : AbstractValidator<ActualizarProductoDto>
    {
        private static readonly string[] TiposIgvValidos = ["10", "20", "30"];

        public ActualizarproductoValidatorDto() 
        {
            When(x => x.NombreComercial != null, () =>
            {
                RuleFor(x => x.NombreComercial!)
                    .NotEmpty().WithMessage("El nombre comercial no puede estar vacío.")
                    .MaximumLength(150);
            });

            When(x => x.RegistroSanitario != null, () =>
            {
                RuleFor(x => x.RegistroSanitario!)
                    .NotEmpty().WithMessage("El Registro Sanitario no puede estar vacío.")
                    .MaximumLength(30)
                    .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("Registro Sanitario no válido.");
            });

            When(x => !string.IsNullOrWhiteSpace(x.CodigoBarras), () =>
            {
                RuleFor(x => x.CodigoBarras!)
                    .MinimumLength(8)
                    .MaximumLength(30)
                    .Matches(@"^[a-zA-Z0-9]+$");
            });

            When(x => x.TipoAfectacionIgvDefault != null, () =>
            {
                RuleFor(x => x.TipoAfectacionIgvDefault!)
                    .Must(tipo => TiposIgvValidos.Contains(tipo))
                    .WithMessage("Tipo de IGV no válido (10, 20, 30).");
            });

            When(x => x.UnidadesPorCaja.HasValue, () =>
            {
                RuleFor(x => x.UnidadesPorCaja!.Value)
                    .GreaterThan(0).WithMessage("Unidades por caja debe ser mayor a 0.");
            });

            When(x => x.UnidadesPorBlister.HasValue, () =>
            {
                RuleFor(x => x.UnidadesPorBlister!.Value)
                    .GreaterThan(0).WithMessage("Unidades por blíster debe ser mayor a 0.");
            });

            When(x => x.PrecioUnidad.HasValue, () =>
            {
                RuleFor(x => x.PrecioUnidad)
                    .GreaterThan(0)
                    .WithMessage("El precio unitario debe ser mayor a 0.");
            });

            When(x => x.PrecioBlister.HasValue, () =>
            {
                RuleFor(x => x.PrecioBlister)
                    .GreaterThan(0)
                    .WithMessage("El precio por blister debe ser mayor a 0.");
            });

            When(x => x.PrecioCaja.HasValue, () =>
            {
                RuleFor(x => x.PrecioCaja)
                    .GreaterThan(0)
                    .WithMessage("El precio por caja debe ser mayor a 0.");
            });
        }
    }
}
