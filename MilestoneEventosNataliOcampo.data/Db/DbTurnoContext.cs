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
    }
}
