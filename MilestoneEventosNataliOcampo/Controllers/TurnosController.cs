using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MilestoneEventosNataliOcampo.data.Modelos;
using MilestoneEventosNataliOcampo.data.Repositorios;
using MilestoneEventosNataliOcampo.web.Models;
using System.Diagnostics;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MilestoneEventosNataliOcampo.web.Controllers
{
    public class TurnosController : Controller
    {
        private ITurnoRepository _turnoRepository;

        public TurnosController(ITurnoRepository turnoRepository)
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

            turnosddbb = turnosddbb.Where(t => t.Borrado == false).ToList();

            Turnos = turnosddbb.Select(t => new TurnosVM { CodigoTurno = t.CodigoTurno, FechaTurno = t.FechaTurno,
                Paciente = new Paciente{Nombre = t.Paciente.Nombre, Apellido = t.Paciente.Apellido ,DNI= t.Paciente.DNI}
                , Especialidad= t.Especialidad.Nombre}).ToList();
            return View(Turnos);
        }

        //detail 
        public ActionResult Details(int CodigoTurno)
        {

            TurnosVM turno = null;

            var turnosddbb = _turnoRepository.ObtenerTurnoId(CodigoTurno);

            turno = new TurnosVM { CodigoTurno = turnosddbb.CodigoTurno,
                Paciente = new Paciente { Nombre = turnosddbb.Paciente.Nombre, Apellido = turnosddbb.Paciente.Apellido, DNI = turnosddbb.Paciente.DNI },
                FechaTurno = turnosddbb.FechaTurno, Especialidad = turnosddbb.Especialidad.Nombre };

            return View(turno);
        }
           
        
        //crear  get 
        public ActionResult Create()
        {
            TurnoAltaVM turno = new TurnoAltaVM();
            turno.Especialidades = _turnoRepository.ObtenerEspecialidades()
              .Select(e => new SelectListItem(e.Nombre, e.Id.ToString()))
              .ToList();


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

                _turnoRepository.AgregarTurno{ turno.FechaTurno = turnoNuevo.FechaTurno, turno.EspecialidadId = turnoNuevo.EspecialidadId, turno.PacienteId = pacienteNuevo.Id}
                ;

                TempData["Mensaje"] = "Turno creado con exito";

                return RedirectToAction(nameof(Index));

            }
            catch
            {
                return View();
            }

        }


        // modificar 
        public ActionResult Edit(int CodigoTurno)
        {
            Turno turnoModificar = null;

            var turnosddbb = _turnoRepository.Modificar(CodigoTurno);
            turnoModificar = new TurnosVM { FechaTurno = turnosddbb.FechaTurno, Especialidad = turnosddbb.EspecialidadId, Paciente = turnosddbb.Paciente };

            return View(turnoModificar);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Edit(int CodigoTurno, TurnosVM turnoNuevo)
        {
            try
            {
                Turno turnoModificar = new Turno();

                turnoModificar = new TurnosVM { FechaTurno = turnoNuevo.FechaTurno, Especialidad = turnoNuevo.EspecialidadId, Paciente = turnoNuevo.Paciente };

                _turnoRepository.Modificar(turnoModificar);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        //"eliminar"
        public ActionResult Delete(int CodigoTurno)
        {
            Turno turno = new Turno();

            var turnosddbb = _turnoRepository.ObtenerTurnoId(CodigoTurno);
            turno = new TurnosVM { FechaTurno = turno.FechaTurno };


            return View(turno);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int CodigoTurno, IFormCollection collection)
        {
            try
            {
                var turnosddbb = _turnoRepository.ObtenerTurnoId(CodigoTurno);
                Turno.Borrado = true;
                _turnoRepository.ActualizarEvento(CodigoTurno);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
      
}