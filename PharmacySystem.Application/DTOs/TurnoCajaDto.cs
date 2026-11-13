using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.DTOs
{
    public class TurnoCajaDto
    {
        public decimal SaldoInicialEfectivo {  get; set; }
        public string? Observaciones { get; set; }
    }

    public class CerrarCajaDto
    {
        public decimal ConteoFisicoEfectivo { get; set; }
        public decimal ConteoFisicoYape { get; set; }
        public decimal ConteoFisicoPlin { get; set; }
        public decimal ConteoFisicoTarjeta { get; set; }
        public string? Observaciones { get; set; }
    }

    public class CorregirConteoTurnoDto
    {        
        public decimal? ConteoFisicoEfectivo { get; set; }
        public decimal? ConteoFisicoYape { get; set; }
        public decimal? ConteoFisicoPlin { get; set; }
        public decimal? ConteoFisicoTarjeta { get; set; }
        public string MotivoCorreccion { get; set; } = string.Empty;
    }

    public class TurnoActivoResponseDto
    {
        public Guid Id { get; set; }
        public Guid SedeId { get; set; }
        public Guid UsuarioId { get; set; }
        public DateTime FechaApertura { get; set; }
        public decimal SaldoInicialEfectivo { get; set; }
        public bool EstadoAbierto { get; set; }
        public string? Observaciones { get; set; }
    }

    public class TurnoCajaResponseDto
    {
        public Guid Id { get; set; }
        public Guid SedeId { get; set; }
        public string NombreSede { get; set; } = string.Empty;
        public Guid UsuarioId { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime FechaApertura { get; set; }
        public DateTime? FechaCierre { get; set; }
        public decimal SaldoInicialEfectivo { get; set; }

        public decimal? ConteoFisicoEfectivo { get; set; }
        public decimal? ConteoFisicoYape { get; set; }
        public decimal? ConteoFisicoPlin { get; set; }
        public decimal? ConteoFisicoTarjeta { get; set; }

        public decimal? SistemaTotalEfectivo { get; set; }
        public decimal? SistemaTotalYape { get; set; }
        public decimal? SistemaTotalPlin { get; set; }
        public decimal? SistemaTotalTarjeta { get; set; }

        public decimal? DiferenciaSobranteFaltante { get; set; }
        public bool EstadoAbierto { get; set; }
        public string? Observaciones { get; set; }
    }
}
