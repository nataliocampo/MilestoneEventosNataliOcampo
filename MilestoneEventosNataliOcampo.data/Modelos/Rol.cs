using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MilestoneEventosNataliOcampo.data.Modelos
{
    public class Rol
    {

        [Key]
        public int RolId { get; }

        [Column(TypeName = "String")]
        public String NombreRol { get; }


    }
}
