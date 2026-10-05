using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MilestoneEventosNataliOcampo.data.Db;
using MilestoneEventosNataliOcampo.data.Modelos;
using System;
using System.Collections.Generic;
using System.Text;

namespace MilestoneEventosNataliOcampo.data.Repositorios
{

    public class TurnoRepository : ITurnoRepository
    {
        private DbTurnoContext _context;

        public TurnoRepository(DbTurnoContext context)
        {
            _context = context;
        }


        public void AgregarPaciente(Paciente paciente)
        {
            _context.Pacientes.Add(paciente);
            _context.SaveChanges();
        }
        public List<Especialidad> ObtenerEspecialidades()
        {
            return _context.Especialidades.ToList();
        }


        public List<Turno> ObtenerTurnos()
        {
            return _context.Turnos
          .Include(t => t.Paciente)
         .Include(t => t.Especialidad)
         .ToList();
        }

        public Turno ObtenerTurnoId(int CodigoTurno)
        {
            return _context.Turnos
          .Include(t => t.Paciente)
         .Include(t => t.Especialidad)
         .FirstOrDefault(t => t.CodigoTurno == CodigoTurno);
        }

        public void AgregarTurno(Turno turno)
        {


            _context.Turnos.Add(turno);
            _context.SaveChanges();


        }

        public void Modificar(Turno turno)
        {
            _context.Turnos.Update(turno);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var turno = _context.Turnos.Find(id);
            if (turno != null)
            {
                _context.Turnos.Remove(turno);
                _context.SaveChanges();
            }

        }
    }
}



