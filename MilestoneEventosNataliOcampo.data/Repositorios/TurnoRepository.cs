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


        public List<TimeOnly> Horarios()
        {
            List<TimeOnly> horarios = new List<TimeOnly>();

            TimeOnly horaI = new TimeOnly(9, 0);
            TimeOnly horaFinal = new TimeOnly(17, 30);

            while (horaI <= horaFinal)
            {
                horarios.Add(horaI);
                horaI = horaI.AddMinutes(30);
            }

            return horarios;
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


        //Buscar si el usuario que inicio ya tenia paciente cargado, lo usa, si no debe crear paciente
        public Paciente? BuscarPacienteXusuario(int usuarioId)
        { 
            return _context.Pacientes.FirstOrDefault(p => p.UsuarioId == usuarioId);
        }

        //Buscar medico disponible para asignar al turno
        public Medico? AsignarMedico(int especialidadIdRequerida, DateTime fechaTurno, TimeOnly horaTurno)
        {
            return _context.Medicos.Where(m => m.EspecialidadId == especialidadIdRequerida
            && !_context.Turnos.Any(t => t.MedicoId == m.Id
                && t.FechaTurno == fechaTurno.Date
                && t.HoraTurno == horaTurno
                && !t.Borrado))
        .OrderBy(m => m.Id)
        .FirstOrDefault();
        }
        

        public List<Turno> ObtenerTurnos()
        {
            return _context.Turnos
          .Include(t => t.Paciente)
         .Include(t => t.Especialidad)
         .Include(t =>t.Medico)
         .ToList();
        }

        public bool FechaValida(DateTime fecha, TimeOnly hora)
        {
            DateTime fechaHoraTurno = fecha.Date + hora.ToTimeSpan();

            return fechaHoraTurno >= DateTime.Now;
        }
        public Turno ObtenerTurnoId(int CodigoTurno)
        {
            return _context.Turnos
          .Include(t => t.Paciente)
         .Include(t => t.Especialidad)
         .Include(t => t.Medico).ThenInclude(m => m.Usuario)
         .FirstOrDefault(t => t.CodigoTurno == CodigoTurno && !t.Borrado);
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
                turno.Borrado = true;
                _context.SaveChanges();
            }

        }
    }
}



