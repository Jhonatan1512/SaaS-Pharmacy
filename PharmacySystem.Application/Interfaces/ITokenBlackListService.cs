using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Application.Interfaces
{
    public interface ITokenBlackListService
    {
        Task IvalidTokenAsync(string token);
        Task<bool> EsTokenInvalidoAsync(string token);
    }
}
