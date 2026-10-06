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


        public DbSet<Turno> Turnos => Set<Turno>();


        public DbSet<Paciente> Pacientes => Set<Paciente>();
        public DbSet<Especialidad> Especialidades => Set<Especialidad>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Especialidad>().HasData(
                new Especialidad { Id = 1, Nombre = "Clínica Médica" },
                new Especialidad { Id = 2, Nombre = "Pediatría" },
                new Especialidad { Id = 3, Nombre = "Cardiología" },
                new Especialidad { Id = 4, Nombre = "Traumatología" },
                new Especialidad { Id = 5, Nombre = "Dermatología" }
            );
        }
    }
}
