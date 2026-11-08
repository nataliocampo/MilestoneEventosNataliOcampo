using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MilestoneEventosNataliOcampo.data.Modelos
{
    public class Permiso
    {

        [Key]
        public int Id { get; }
        public String NombrePermiso { get; set; }
          

    }

}

    }
}
