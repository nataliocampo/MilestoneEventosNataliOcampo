using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text;

namespace MilestoneEventosNataliOcampo.data.Modelos
{
    internal class Paciente
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int DNI { get; set; }
        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }
        [Required]
        [StringLength(50)]
        public string Apellido { get; set; }


    }
}
