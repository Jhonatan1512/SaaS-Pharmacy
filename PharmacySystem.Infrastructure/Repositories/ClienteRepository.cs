using Microsoft.EntityFrameworkCore;
using PharmacySystem.Domain.Entities;
using PharmacySystem.Domain.Interfaces;
using PharmacySystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmacySystem.Infrastructure.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly ApplicationDbContext _context;
        public ClienteRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task Actualizar(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task CrearAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task Eliminar(Cliente cliente)
        {
            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExisteDocumentoAsync(TipoDoc tipoDoc, string numeroDocumento, Guid? ignorarId = null)
        {
            return await _context.Clientes
                .AnyAsync(c => c.TipoDocumento == tipoDoc &&
                                c.NumeroDocumento == numeroDocumento &&
                                (!ignorarId.HasValue || c.Id != ignorarId.Value));
        }

        public async Task<Cliente?> ObtenerPorDocumentoAsync(TipoDoc tipoDoc, string numeroDocumento)
        {
            return await _context.Clientes
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.TipoDocumento == tipoDoc && c.NumeroDocumento == numeroDocumento);
        }

        public async Task<Cliente?> ObtenerPorIdAsync(Guid id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        public async Task<IEnumerable<Cliente>> ObtenerTodosAsync(string? busqueda = null)
        {
            var query = _context.Clientes.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                busqueda = busqueda.Trim().ToLower();
                query = query.Where(c =>
                    c.NumeroDocumento.Contains(busqueda) ||
                    c.NombresRazonSocial.ToLower().Contains(busqueda));
            }

            return await query
                .OrderBy(c => c.NombresRazonSocial)
                .ToListAsync();
        }
    }
}
