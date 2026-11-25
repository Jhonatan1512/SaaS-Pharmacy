using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface IPlantillaSolicitudService
    {
        string GenerarCartaFormal(SolicitudReposicion solicitud);
    }
}
