using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace MilestoneEventosNataliOcampo.web.Models

{
    public class TurnosVM
    {
        public int CodigoTurno { get; set; }
        public PacienteVM Paciente { get; set; }
        public DateTime FechaTurno{ get; set; }

        public string Especialidad { get; set; }    
    }

    public class TurnoAltaVM
    {
        public int CodigoTurno { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]

        public PacienteVM Paciente  { get; set; }

        [Required(ErrorMessage = "Campo Requerido")]
        public DateTime FechaTurno { get; set; }


        [Required(ErrorMessage = "Campo Requerido")]
        public int EspecialidadId { get; set; }      

        public List<SelectListItem> Especialidades { get; set; }
    }

    public class PacienteVM
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public int DNI { get; set; }


    }
}