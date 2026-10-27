using Microsoft.EntityFrameworkCore;
using GobernanzaDigital.Models;

namespace GobernanzaDigital.Data
{
    public class GobernanzaContext : DbContext
    {
        public GobernanzaContext(DbContextOptions<GobernanzaContext> options) : base(options) { }

        public DbSet<Contribuyente> Contribuyentes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<ContribuyenteServicio> ContribuyenteServicios { get; set; }
        public DbSet<Boleta> Boletas { get; set; }
        public DbSet<Pasarela> Pasarelas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // No puede haber dos contribuyentes con el mismo DNI/CUIT
            modelBuilder.Entity<Contribuyente>()
                .HasIndex(c => c.Identificacion)
                .IsUnique();

            // Un contribuyente no puede tener dos veces el mismo servicio asignado
            modelBuilder.Entity<ContribuyenteServicio>()
                .HasIndex(cs => new { cs.ContribuyenteId, cs.ServicioId })
                .IsUnique();

            // El código de pago es único en todo el sistema
            modelBuilder.Entity<Boleta>()
                .HasIndex(b => b.CodigoPago)
                .IsUnique();

            // Evita boletas duplicadas: misma persona + mismo servicio + mismo período
            modelBuilder.Entity<Boleta>()
                .HasIndex(b => new { b.ContribuyenteId, b.ServicioId, b.Periodo })
                .IsUnique();

            // No se puede borrar un contribuyente o servicio que tenga boletas
            modelBuilder.Entity<Boleta>()
                .HasOne(b => b.Contribuyente)
                .WithMany(c => c.Boletas)
                .HasForeignKey(b => b.ContribuyenteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Boleta>()
                .HasOne(b => b.Servicio)
                .WithMany()
                .HasForeignKey(b => b.ServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Los enums se guardan como texto ("Pendiente") y no como número (0)
            modelBuilder.Entity<Contribuyente>()
                .Property(c => c.Tipo)
                .HasConversion<string>();

            modelBuilder.Entity<Boleta>()
                .Property(b => b.Estado)
                .HasConversion<string>();
        }
    }
}