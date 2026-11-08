using System;
using System.Collections.Generic;
using System.Text;

namespace MilestoneEventosNataliOcampo.data.Modelos
{
    public class Usuario
    {

        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public int RolId { get; set; }
        public Rol Rol { get; set; } = null!;

        public DateTime FechaAlta { get; set; }

    }
}


