using FluentValidation;
using PharmacySystem.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Validatos
{
    public class RegistrarProductoConLoteValidatorDto : AbstractValidator<RegistrarProductoConLoteDto>
    {
        public RegistrarProductoConLoteValidatorDto()
        {
            RuleFor(x => x.CodigoBarras)
            .NotEmpty().WithMessage("El código de barras es obligatorio.")
            .MaximumLength(50).WithMessage("El código de barras no puede exceder 50 caracteres.");

            RuleFor(x => x.NombreComercial)
                .NotEmpty().WithMessage("El nombre comercial es obligatorio.")
                .MaximumLength(200).WithMessage("El nombre comercial no puede exceder 200 caracteres.");

            RuleFor(x => x.PrincipioActivo)
                .NotEmpty().WithMessage("El principio activo es obligatorio.")
                .MaximumLength(200);

            RuleFor(x => x.Presentacion)
                .NotEmpty().WithMessage("La presentación es obligatoria.")
                .MaximumLength(150);

            RuleFor(x => x.RegistroSanitario)
                .NotEmpty().WithMessage("El registro sanitario es obligatorio.")
                .MaximumLength(50);

            RuleFor(x => x.TipoAfectacionIgvDefault)
            .Must(t => t == "10" || t == "20" || t == "30")
            .WithMessage("El tipo de afectación IGV debe ser '10' (Gravado), '20' (Exonerado) o '30' (Inafecto).");

            // Factores de conversión
            RuleFor(x => x.UnidadesPorCaja)
                .GreaterThan(0).WithMessage("Las unidades por caja deben ser mayores a 0.");

            RuleFor(x => x.UnidadesPorBlister)
                .GreaterThanOrEqualTo(0).WithMessage("Las unidades por blíster deben ser menores a 0.")
                .LessThanOrEqualTo(x => x.UnidadesPorCaja)
                .WithMessage("Las unidades por blíster no pueden superar las unidades por caja.");

            // Precios de venta referenciales
            RuleFor(x => x.PrecioUnidad)
                .GreaterThan(0).WithMessage("El precio por unidad debe ser mayor a 0.");

            RuleFor(x => x.PrecioBlister)
                .GreaterThanOrEqualTo(0).WithMessage("El precio por blíster no puede ser negativo.");

            RuleFor(x => x.PrecioCaja)
                .GreaterThanOrEqualTo(0).WithMessage("El precio por caja no puede ser negativo.");

            // Lote e ingreso al almacén
            RuleFor(x => x.CodigoLote)
                .NotEmpty().WithMessage("El código de lote es obligatorio.")
                .MaximumLength(50).WithMessage("El código de lote no puede exceder 50 caracteres.");

            RuleFor(x => x.FechaVencimiento)
            .GreaterThan(DateTime.UtcNow.Date)
            .WithMessage("La fecha de vencimiento del lote debe ser posterior a la fecha actual.");

            RuleFor(x => x.PrecioCostoUnitario)
                .GreaterThan(0).WithMessage("El precio de costo unitario debe ser mayor a 0.");

            RuleFor(x => x.CantidadUnidades)
                .GreaterThan(0).WithMessage("La cantidad de unidades que ingresan debe ser mayor a 0.");

            RuleFor(x => x.StockMinimoAlmacenCentral)
                .GreaterThanOrEqualTo(0).WithMessage("El stock mínimo no puede ser negativo.");

            RuleFor(x => x.ReferenciaDocumento)
                .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.ReferenciaDocumento));
        }
    }
}
