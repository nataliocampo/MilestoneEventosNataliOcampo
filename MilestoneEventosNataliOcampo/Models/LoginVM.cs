using System.ComponentModel.DataAnnotations;

namespace MilestoneEventosNataliOcampo.web.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        public string Email { get; set; } = null!;


        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}

