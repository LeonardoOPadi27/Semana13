using System;
using System.Collections.Generic;
using Gimnasio.Entidades;
using Microsoft.EntityFrameworkCore;

namespace Gimnasio.Datos.Contexto;

public partial class GimnasioContext : DbContext
{
    public GimnasioContext(DbContextOptions<GimnasioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Membresia> Membresia { get; set; }

    public virtual DbSet<Socio> Socio { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Membresia>(entity =>
        {
            entity.HasKey(e => e.IdMembresia);

            entity.HasIndex(e => e.Nombre, "UQ_Membresia_Nombre").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Membresia_Activo");
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.PrecioMensual).HasColumnType("decimal(8, 2)");
        });

        modelBuilder.Entity<Socio>(entity =>
        {
            entity.HasKey(e => e.IdSocio);

            entity.HasIndex(e => e.Dni, "UQ_Socio_Dni").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true, "DF_Socio_Activo");
            entity.Property(e => e.Apellidos).HasMaxLength(60);
            entity.Property(e => e.Dni)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.FechaInscripcion).HasDefaultValueSql("(CONVERT([date],getdate()))", "DF_Socio_FechaInscripcion");
            entity.Property(e => e.Nombres).HasMaxLength(60);
            entity.Property(e => e.Telefono).HasMaxLength(15);

            entity.HasOne(d => d.IdMembresiaNavigation).WithMany(p => p.Socio)
                .HasForeignKey(d => d.IdMembresia)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Socio_Membresia");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
