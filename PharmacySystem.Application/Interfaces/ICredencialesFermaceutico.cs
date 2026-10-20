using PharmacySystem.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface ICredencialesFermaceutico
    {
        Task ValidarRegistrosDuplicados(string email, string dni, string ruc);
    }
}
