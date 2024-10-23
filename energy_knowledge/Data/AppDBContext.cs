using Microsoft.EntityFrameworkCore;
using energy_knowledge.Models;

namespace energy_knowledge.Data
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>(tb =>
            {
                tb.HasKey(col => col.IdUsuario);
                tb.Property(col => col.IdUsuario).UseIdentityColumn().ValueGeneratedOnAdd();

                tb.Property(col => col.Nombre).HasMaxLength(50);
                tb.Property(col => col.Apellidos).HasMaxLength(50);
                tb.Property(col => col.Correo).HasMaxLength(50);
                tb.Property(col => col.Contrasenia).HasMaxLength(50);
            });

            modelBuilder.Entity<Usuario>().ToTable("Usuario");
        }
    }
}
