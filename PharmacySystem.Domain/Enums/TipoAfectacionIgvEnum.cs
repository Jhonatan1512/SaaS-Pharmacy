using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Domain.Enums
{
    public enum TipoAfectacionIgvEnum
    {
        Gravado = 10,    // Incluye 18% IGV
        Exonerado = 20,  // Medicamentos exonerados por ley (0% IGV)
        Inafecto = 30    // 0% IGV
    }
}
