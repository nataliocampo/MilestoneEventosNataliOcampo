using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;


namespace MilestoneEventosNataliOcampo.data.Modelos
{
    [PrimaryKey("RolId", "PermisoId")]

    public class RolPermiso
    {
            [ForeignKey("RolId")]
            public Rol Rol { get; set; }

            [ForeignKey("PermisoId")]
            public Permiso Permiso { get; set; }

            public int RolId { get; set; }


            public int PermisoId { get; set; }


        }
    }
