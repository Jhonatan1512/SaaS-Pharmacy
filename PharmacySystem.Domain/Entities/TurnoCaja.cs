using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Entities
{
    public class TurnoCaja
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid SedeId { get; set; }
        public Guid UsuarioId { get; set; }
        public DateTime FechaApertura { get; set; } = DateTime.UtcNow;
        public DateTime? FechaCierre { get; set; }
        public decimal SaldoInicialEfectivo { get; set; }

        // Cierre Ciego
        public decimal? ConteoFisicoEfectivo { get; set; }
        public decimal? ConteoFisicoYape { get; set; }
        public decimal? ConteoFisicoPlin { get; set; }
        public decimal? ConteoFisicoTarjeta { get; set; }

        // Valores teóricos calculados por el Sistema
        public decimal? SistemaTotalEfectivo { get; set; }
        public decimal? SistemaTotalYape { get; set; }
        public decimal? SistemaTotalPlin { get; set; }
        public decimal? SistemaTotalTarjeta { get; set; }

        public decimal? DiferenciaSobranteFaltante { get; set; }
        public bool EstadoAbierto { get; set; } = true;
        public string? Observaciones { get; set; }

        public Sede Sede { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
        public ICollection<Venta> Ventas { get; set; } = new List<Venta>();

        public static TurnoCaja Abrir(Guid sedeId, Guid usuarioId, decimal saldoInicialEfectivo, string? observaciones = null)
        {
            if (saldoInicialEfectivo < 0) throw new ArgumentException("El saldo inicial en efectivo no puede ser negativo");

            return new TurnoCaja
            {
                Id = Guid.NewGuid(),
                SedeId = sedeId,
                UsuarioId = usuarioId,
                FechaApertura = DateTime.UtcNow,
                SaldoInicialEfectivo = saldoInicialEfectivo,
                EstadoAbierto = true,
                Observaciones = observaciones
            };
        }

        public void CerrarTurno(
        decimal conteoEfectivo,
        decimal conteoYape,
        decimal conteoPlin,
        decimal conteoTarjeta,
        decimal sistemaEfectivo,
        decimal sistemaYape,
        decimal sistemaPlin,
        decimal sistemaTarjeta,
        string? observacionesCierre = null)
        {
            if (!EstadoAbierto)
                throw new InvalidOperationException("Este turno de caja ya se encuentra cerrado.");

            // Registrar conteo físico
            ConteoFisicoEfectivo = conteoEfectivo;
            ConteoFisicoYape = conteoYape;
            ConteoFisicoPlin = conteoPlin;
            ConteoFisicoTarjeta = conteoTarjeta;

            // Registrar totales teóricos de ventas del sistema
            SistemaTotalEfectivo = sistemaEfectivo;
            SistemaTotalYape = sistemaYape;
            SistemaTotalPlin = sistemaPlin;
            SistemaTotalTarjeta = sistemaTarjeta;

            // Calcular diferencia global (Total Declarado vs Total Esperado en Caja)
            decimal totalDeclarado = conteoEfectivo + conteoYape + conteoPlin + conteoTarjeta;
            decimal efectivoEsperado = SaldoInicialEfectivo + sistemaEfectivo;
            decimal totalEsperado = efectivoEsperado + sistemaYape + sistemaPlin + sistemaTarjeta;

            DiferenciaSobranteFaltante = totalDeclarado - totalEsperado;
            FechaCierre = DateTime.UtcNow;
            EstadoAbierto = false;

            if (!string.IsNullOrWhiteSpace(observacionesCierre))
            {
                Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                    ? $"[CIERRE]: {observacionesCierre}"
                    : $"{Observaciones} | [CIERRE]: {observacionesCierre}";
            }
        }

        public void CorregirConteoCierre(
    decimal? nuevoEfectivo,
    decimal? nuevoYape,
    decimal? nuevoPlin,
    decimal? nuevoTarjeta,
    string motivoCorreccion)
        {
            if (EstadoAbierto)
                throw new InvalidOperationException("No se puede corregir el conteo de un turno que aún sigue abierto.");

            if (string.IsNullOrWhiteSpace(motivoCorreccion))
                throw new ArgumentException("Debe especificar el motivo de la corrección.");

            // Actualizar solo los montos 
            if (nuevoEfectivo.HasValue) ConteoFisicoEfectivo = nuevoEfectivo.Value;
            if (nuevoYape.HasValue) ConteoFisicoYape = nuevoYape.Value;
            if (nuevoPlin.HasValue) ConteoFisicoPlin = nuevoPlin.Value;
            if (nuevoTarjeta.HasValue) ConteoFisicoTarjeta = nuevoTarjeta.Value;

            // Recalcular diferencia usando los totales teóricos ya congelados al cierre
            decimal totalDeclarado =
                (ConteoFisicoEfectivo ?? 0m) +
                (ConteoFisicoYape ?? 0m) +
                (ConteoFisicoPlin ?? 0m) +
                (ConteoFisicoTarjeta ?? 0m);

            decimal totalEsperado =
                SaldoInicialEfectivo +
                (SistemaTotalEfectivo ?? 0m) +
                (SistemaTotalYape ?? 0m) +
                (SistemaTotalPlin ?? 0m) +
                (SistemaTotalTarjeta ?? 0m);

            DiferenciaSobranteFaltante = totalDeclarado - totalEsperado;

            // Registrar trazabilidad en observaciones
            string registroAuditoria = $"[CORRECCIÓN CONTEO]: {motivoCorreccion.Trim()}";
            Observaciones = string.IsNullOrWhiteSpace(Observaciones)
                ? registroAuditoria
                : $"{Observaciones} | {registroAuditoria}";
        }

    }

} 
