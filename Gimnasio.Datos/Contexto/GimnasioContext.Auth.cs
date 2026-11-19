using Gimnasio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Contexto;

public partial class GimnasioContext
{
    public virtual DbSet<Usuario> Usuario { get; set; }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(usuario => usuario.IdUsuario);
            entity.HasIndex(usuario => usuario.Correo, "UQ_Usuario_Correo").IsUnique();
            entity.Property(usuario => usuario.Correo).HasMaxLength(100);
            entity.Property(usuario => usuario.ClaveHash).HasMaxLength(100);
            entity.Property(usuario => usuario.Rol).HasMaxLength(20);
        });
    }
}