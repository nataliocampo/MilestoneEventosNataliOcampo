using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;


namespace MilestoneEventosNataliOcampo.data.Modelos
{
    public class Turno
    {
        [Key]
        public int CodigoTurno { get; set; }

        [Column(TypeName = "Date")]
        public DateTime FechaTurno { get; set; }

        [Required]
        public int EspecialidadId { get; set; }

        [ForeignKey("EspecialidadId")]
        public Especialidad Especialidad { get; set; }

        [Required]
        public int PacienteId { get; set; }

        [ForeignKey("PacienteId")]
        public Paciente Paciente { get; set; }

        [Column(TypeName = "bit")]
        public bool Borrado { get; set; }
    }

}

