using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MilestoneEventosNataliOcampo.data.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace MilestoneEventosNataliOcampo.data.Db
{
    public class DbTurnoContext : DbContext
    {

        public DbTurnoContext(DbContextOptions<DbTurnoContext> options) : base(options) { }

            public DbSet<Medico> Medicos => Set<Medico>();
            public DbSet<Usuario> Usuarios => Set<Usuario>();

            public DbSet<RolPermiso> RolPermiso => Set<RolPermiso>();

            public DbSet<Rol> Rol => Set<Rol>();

            public DbSet<Permiso> Permiso => Set<Permiso>();

            public DbSet<Turno> Turnos => Set<Turno>();


        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<Especialidad> Especialidades => Set<Especialidad>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Turno>()
                    .HasOne(t => t.Medico)
                    .WithMany(m => m.Turnos)
                    .HasForeignKey(t => t.MedicoId)
                    .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Especialidad>().HasData(
                new Especialidad { Id = 1, Nombre = "Clínica Médica" },
                new Especialidad { Id = 2, Nombre = "Pediatría" },
                new Especialidad { Id = 3, Nombre = "Cardiología" },
                new Especialidad { Id = 4, Nombre = "Traumatología" },
                new Especialidad { Id = 5, Nombre = "Dermatología" }
            );
            modelBuilder.Entity<Rol>().HasData(
                new Rol { RolId = 1, NombreRol = "Medico" },
                new Rol { RolId = 2, NombreRol = "Paciente" }
                );

            modelBuilder.Entity<Permiso>().HasData(
                new Permiso { Id = 1, NombrePermiso = "Turnos.VerTodos" },
                new Permiso { Id = 2, NombrePermiso = "Turnos.Crear" },
                new Permiso { Id = 3, NombrePermiso = "Turnos.Editar" },
                new Permiso { Id = 4, NombrePermiso = "Turnos.Eliminar" },
                new Permiso { Id = 5, NombrePermiso = "Turnos.VerPropios" }
                 );


            modelBuilder.Entity<RolPermiso>().HasData(
                new RolPermiso { PermisoId = 1, RolId = 1 },
                new RolPermiso { PermisoId = 2, RolId = 2 },
                new RolPermiso { PermisoId = 3, RolId = 2 },
                new RolPermiso { PermisoId = 4, RolId = 2 },
                new RolPermiso { PermisoId = 5, RolId = 2 }
                );

            modelBuilder.Entity<Medico>()
                .HasIndex(m => m.UsuarioId).IsUnique();

            modelBuilder.Entity<Paciente>()
                .HasIndex(p => p.UsuarioId).IsUnique();

            // No acepta pacientes con el mismo DNI
            modelBuilder.Entity<Paciente>()
                .HasIndex(p => p.DNI).IsUnique();
        }


    
    }
}
