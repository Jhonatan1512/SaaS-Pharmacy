using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PharmacySystem.Domain.Entities;

namespace PharmacySystem.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<Usuario, Role, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }     
        
        public DbSet<Sede> Sedes => Set<Sede>();
        public DbSet<SerieComprobante> SeriesComprobantes => Set<SerieComprobante>();
        //
        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Lote> Lotes => Set<Lote>();
        public DbSet<InventarioSede> InventarioSedes => Set<InventarioSede>();
        public DbSet<ProductoSedeConfig> ProductosSedeConfig => Set<ProductoSedeConfig>();
        //
        public DbSet<SolicitudReposicion> SolicitudesReposicion => Set<SolicitudReposicion>();
        public DbSet<DetalleSolicitudReposicion> DetallesSolicitudReposicion => Set<DetalleSolicitudReposicion>();
        //
        public DbSet<Cliente> Clientes => Set<Cliente>();
        public DbSet<Venta> Ventas => Set<Venta>();
        public DbSet<DetalleVenta> DetallesVenta => Set<DetalleVenta>();
        public DbSet<PagoVenta> PagosVenta => Set<PagoVenta>();
        public DbSet<TurnoCaja> TurnosCaja => Set<TurnoCaja>();
        //
        public DbSet<TransaccionKardex> TransaccionesKardex => Set<TransaccionKardex>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            foreach (var property in builder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()).Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                property.SetColumnType("decimal(18,4)");
            }

            builder.Entity<Venta>(v =>
            {
                v.Property(x => x.MontoOperacionesGravadas).HasColumnType("decimal(18,2)");
                v.Property(x => x.MontoOperacionesExoneradas).HasColumnType("decimal(18,2)");
                v.Property(x => x.MontoOperacionesInafectas).HasColumnType("decimal(18,2)");
                v.Property(x => x.MontoIgv).HasColumnType("decimal(18,2)");
                v.Property(x => x.MontoTotalImpuestos).HasColumnType("decimal(18,2)");
                v.Property(x => x.MontoTotalVenta).HasColumnType("decimal(18,2)");
            });

            builder.Entity<Usuario>().HasOne(u => u.Sede).WithMany(u => u.Usuarios).HasForeignKey(u => u.SedeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Usuario>().HasIndex(u => u.NumeroDoc).IsUnique();
            builder.Entity<Cliente>().HasIndex(c => c.NumeroDocumento).IsUnique();

            //Inventarios
            builder.Entity<InventarioSede>().HasOne(i => i.Sede).WithMany(s => s.Inventarios).HasForeignKey(i => i.SedeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<InventarioSede>().HasOne(i => i.Lote).WithMany(l => l.InventariosSede).HasForeignKey(i => i.LoteId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<InventarioSede>().HasOne(i => i.UsuarioRecibe).WithMany().HasForeignKey(i => i.UsuarioRecibeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ProductoSedeConfig>().HasOne(p => p.Sede).WithMany(s => s.ProductosConfig).HasForeignKey(p => p.SedeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<ProductoSedeConfig>().HasOne(p => p.Producto).WithMany().HasForeignKey(p => p.ProductoId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<SolicitudReposicion>().HasOne(s => s.SedeSolicitante).WithMany().HasForeignKey(s => s.SedeSolicitanteId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<SolicitudReposicion>().HasOne(s => s.UsuarioSolicitante).WithMany().HasForeignKey(s => s.UsuarioSolicitanteId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<DetalleSolicitudReposicion>().HasOne(d => d.Solicitud).WithMany(s => s.Detalles).HasForeignKey(d => d.SolicitudId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<DetalleSolicitudReposicion>().HasOne(d => d.Producto).WithMany().HasForeignKey(d => d.ProductoId).OnDelete(DeleteBehavior.Restrict);

            // --- Venta ---
            builder.Entity<Venta>().HasOne(v => v.Sede).WithMany().HasForeignKey(v => v.SedeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Venta>().HasOne(v => v.Usuario).WithMany().HasForeignKey(v => v.UsuarioId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Venta>().HasOne(v => v.Cliente).WithMany().HasForeignKey(v => v.ClienteId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Venta>().HasOne(v => v.TurnoCaja).WithMany(t => t.Ventas).HasForeignKey(v => v.TurnoCajaId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<DetalleVenta>().HasOne(d => d.Venta).WithMany(v => v.Detalles).HasForeignKey(d => d.VentaId).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<DetalleVenta>().HasOne(d => d.InventarioSede).WithMany().HasForeignKey(d => d.InventarioSedeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<PagoVenta>().HasOne(p => p.Venta).WithMany(v => v.Pagos).HasForeignKey(p => p.VentaId).OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TurnoCaja>().HasOne(t => t.Sede).WithMany().HasForeignKey(t => t.SedeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<TurnoCaja>().HasOne(t => t.Usuario).WithMany().HasForeignKey(t => t.UsuarioId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TransaccionKardex>().HasOne(k => k.Sede).WithMany().HasForeignKey(k => k.SedeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<TransaccionKardex>().HasOne(k => k.Lote).WithMany().HasForeignKey(k => k.LoteId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<TransaccionKardex>().HasOne(k => k.Usuario).WithMany().HasForeignKey(k => k.UsuarioId).OnDelete(DeleteBehavior.Restrict);

            //Configuración de índices
            builder.Entity<Producto>()
            .HasIndex(p => p.CodigoBarras)
            .HasDatabaseName("IX_Productos_CodigoBarras");

            builder.Entity<Lote>()
                .HasIndex(l => l.CodigoLote)
                .HasDatabaseName("IX_Lotes_CodigoLote");

            builder.Entity<Cliente>()
                .HasIndex(c => c.NumeroDocumento)
                .HasDatabaseName("IX_Clientes_NumeroDocumento");

            // Índice para no repetir correlativos de serie en la misma sede
            builder.Entity<SerieComprobante>()
                .HasIndex(s => new { s.SedeId, s.TipoComprobante, s.Serie })
                .IsUnique()
                .HasDatabaseName("IX_SeriesComprobantes_Sede_Tipo_Serie");

            // Índice de comprobantes de venta
            builder.Entity<Venta>()
                .HasIndex(v => new { v.SedeId, v.TipoComprobante, v.Serie, v.Numero })
                .HasDatabaseName("IX_Ventas_Comprobante");

            SeedIdentityData(builder);
        }

        private static void SeedIdentityData(ModelBuilder builder)
        {
            var roleGerenteId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var roleFarmaceuticoId = Guid.Parse("22222222-2222-2222-2222-222222222222");
            var gerenteUserId = Guid.Parse("33333333-3333-3333-3333-333333333333");

            builder.Entity<Role>().HasData(
                new Role
                {
                    Id = roleGerenteId,
                    Name = "Gerente",
                    NormalizedName = "GERENTE",
                    ConcurrencyStamp = "a1b2c3d4-e5f6-7890-abcd-ef1234567890",
                    Descripcion = "Gerente General / Administrador Central del Sistema"
                },
                new Role
                {
                    Id = roleFarmaceuticoId,
                    Name = "Farmaceutico",
                    NormalizedName = "FARMACEUTICO",
                    ConcurrencyStamp = "f6e5d4c3-b2a1-0987-dcba-fe0987654321",
                    Descripcion = "Farmacéutico Encargado de Sede"
                }
            );

            var gerenteUser = new Usuario
            {
                Id = gerenteUserId,
                UserName = "gerencia@farmacia.com",
                NormalizedUserName = "GERENCIA@FARMACIA.COM",
                Email = "gerencia@farmacia.com",
                NormalizedEmail = "GERENCIA@FARMACIA.COM",
                EmailConfirmed = true,
                Documento = TipoDocumento.RUC,
                NumeroDoc = "20552103816",
                NombreCompletoRazonSocial = "Farmacias Perú S.A.C.",
                SedeId = null, 
                Activo = true,
                SecurityStamp = "d3b07384-d113-40e4-a720-78fbf2fb1873",
                ConcurrencyStamp = "c93d22b2-652f-412f-a6e5-39bc0262e3d3"
            };

            var passwordHasher = new PasswordHasher<Usuario>();
            gerenteUser.PasswordHash = passwordHasher.HashPassword(gerenteUser, "Gerente123*");

            // Insertar Usuario Gerente
            builder.Entity<Usuario>().HasData(gerenteUser);

            // Asignar el Rol "Gerente" a la cuenta del Gerente
            builder.Entity<IdentityUserRole<Guid>>().HasData(new IdentityUserRole<Guid>
            {
                RoleId = roleGerenteId,
                UserId = gerenteUserId
            });
        }
    }
}
