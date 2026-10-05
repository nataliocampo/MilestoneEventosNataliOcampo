using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using MilestoneEventosNataliOcampo.data.Modelos;

namespace MilestoneEventosNataliOcampo.data.Repositorios
{
    public interface ITurnoRepository
    {
       void AgregarTurno(Turno turno);
        void AgregarPaciente(Paciente paciente);
        List<Especialidad> ObtenerEspecialidades();
        Turno ObtenerTurnoId(int CodigoTurno);
        List<Turno> ObtenerTurnos();
        void Modificar(Turno turno);
        void Eliminar(int id);

    }
}
