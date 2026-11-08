using System;
using System.Collections.Generic;
using System.Text;

namespace MilestoneEventosNataliOcampo.data.Modelos
{
    public class Medico
    {
        public int Id { get; set; }


        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public int EspecialidadId { get; set; }
        public Especialidad Especialidad { get; set; } = null!;

        public List<Turno> Turnos { get; set; } = new();
    }
}

