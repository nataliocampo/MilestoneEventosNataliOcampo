using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MilestoneEventosNataliOcampo.data.Modelos;
using MilestoneEventosNataliOcampo.data.Repositorios;
using MilestoneEventosNataliOcampo.web.Models;
using System.Diagnostics;
using System.Xml.Linq;


namespace MilestoneEventosNataliOcampo.web.Controllers
{
    public class TurnosController
    {
        public class EventosController : Controller
        {
           
            private ITurnoRepository _turnoRepository;

            public EventosController(ITurnoRepository turnoRepository)
            {

                _turnoRepository = turnoRepository;
            }
                //mostrar-index
                 public ActionResult Index()
            {
   
                ViewBag.Mensaje = "Inicio";

                List<TurnosVM> Turnos = new List<TurnosVM>();
                //agreagr return en interface
                var turnosddbb = _turnoRepository.ObtenerTurnos();

                turnosddbb = turnosddbb.Where(x => x.Borrado == false).ToList();
                 Turnos = turnosddbb.Select(x => new TurnosVM { CodigoTurno = x.CodigoTurno, FechaTurno = x.FechaTurno, Paciente = x.Paciente }).ToList();
                return View(Turnos);
            }

            //detail 
            public ActionResult Details( int id)
            {

                TurnosVM turno = null;

                var turnosddbb  = _turnoRepository.ObtenerTurnoId(id);
                turno = new TurnosVM { CodigoTurno = turnosddbb.CodigoTurno, Paciente = turnosddbb.Paciente, FechaTurno = turnosddbb.FechaEvento, Especialidad = turnosddbb.Especialidad.Nombre };
                return View(turno);
            }
            //crear  get 
            public ActionResult Create()
            {
                TurnoAltaVM turno = new TurnoAltaVM();
               
                return View(turno);
            }

            //crear post 
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create(TurnoAltaVM turnoNuevo)
            {
                if (!ModelState.IsValid)
                {
                    turnoNuevo.Especialidades = _turnoRepository.ObtenerEspecialidades()
                        .Select(e => new SelectListItem(e.Nombre, e.Id.ToString()))
                        .ToList();


                    ViewBag.Mensaje = "Error - debe  completar los campos requeridos";
                    return View(turnoNuevo);
                }
                try
                {
                    Paciente pacienteNuevo = new Paciente();
                    pacienteNuevo.Nombre = turnoNuevo.Paciente.Nombre;
                    pacienteNuevo.Apellido = turnoNuevo.Paciente.Apellido;
                    pacienteNuevo.DNI = turnoNuevo.Paciente.DNI;

                    _turnoRepository.AgregarPaciente(pacienteNuevo);


                    Turno turno = new Turno();
                    turno.FechaTurno = turnoNuevo.FechaTurno;
                    turno.EspecialidadId = turnoNuevo.EspecialidadId;
                    turno.PacienteId = pacienteNuevo.Id;

                    _turnoRepository.AgregarTurno(turno);

                    TempData["Mensaje"] = "Turno creado con exito";

                    return RedirectToAction(nameof(Index));

                }
                catch
                {
                    return View();
                }
              
            }


            // modificar 


            //"eliminar"


        }
        
    }
}