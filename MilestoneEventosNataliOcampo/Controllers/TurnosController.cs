using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MilestoneEventosNataliOcampo.data.Modelos;
using MilestoneEventosNataliOcampo.data.Repositorios;
using MilestoneEventosNataliOcampo.web.Models;
using System.Diagnostics;
using System.Security.Claims;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace MilestoneEventosNataliOcampo.web.Controllers
{
    public class TurnosController : Controller
    {
        private ITurnoRepository _turnoRepository;
        private IUsuarioRepository _usuarioRepository;

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

            Turnos = turnosddbb.Select(t => new TurnosVM
            {
                CodigoTurno = t.CodigoTurno,
                FechaTurno = t.FechaTurno,
                Paciente = new PacienteVM { Nombre = t.Paciente.Nombre, Apellido = t.Paciente.Apellido, DNI = t.Paciente.DNI }
                ,
                Especialidad = t.Especialidad.Nombre
            }).ToList();
            return View(Turnos);
        }

        //detail 
        public ActionResult Details(int CodigoTurno)
        {

            TurnosVM turno = null;

            var turnosddbb = _turnoRepository.ObtenerTurnoId(CodigoTurno);

            turno = new TurnosVM
            {
                CodigoTurno = turnosddbb.CodigoTurno,
                Paciente = new PacienteVM { Nombre = turnosddbb.Paciente.Nombre, Apellido = turnosddbb.Paciente.Apellido, DNI = turnosddbb.Paciente.DNI },
                FechaTurno = turnosddbb.FechaTurno,
                Especialidad = turnosddbb.Especialidad.Nombre
            };

            return View(turno);
        }


        //crear  get 
        public ActionResult Create()
        {
            TurnoAltaVM turno = new TurnoAltaVM();
            turno.Especialidades = _turnoRepository.ObtenerEspecialidades()
              .Select(e => new SelectListItem(e.Nombre, e.Id.ToString()))
              .ToList();

            turno.Horarios = _turnoRepository.Horarios()
            .Select(h => new SelectListItem(h.ToString("HH:mm"), h.ToString("HH:mm")))
            .ToList();

            //devuelve a la vista la fecha de hoy 
            turno.FechaTurno = DateTime.Today;

            return View(turno);
        }

        private void CargarSelector(TurnoAltaVM turnovm, int usuarioId)
        {
            turnovm.Especialidades = _turnoRepository.ObtenerEspecialidades()
                .Select(e => new SelectListItem(e.Nombre, e.Id.ToString()))
                .ToList();

            turnovm.Horarios = _turnoRepository.Horarios()
                .Select(h => new SelectListItem(h.ToString("HH:mm"), h.ToString("HH:mm")))
                .ToList();

            //traer id para cargar vistas
            var paciente = _turnoRepository.BuscarPacienteXusuario(usuarioId);
            ViewBag.PacienteExiste = paciente != null;
            ViewBag.PacienteNombre = paciente != null ? $"{paciente.Nombre} {paciente.Apellido}" : null;
        
        }


        //crear post 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TurnoAltaVM turnoNuevo)
        {

            if (!int.TryParse(User.FindFirstValue("Id"), out int usuarioId))
            {
                return RedirectToAction("Index", "Home");
            }

            Paciente? paciente = _turnoRepository.BuscarPacienteXusuario(usuarioId);

            if (paciente!= null)
            {
                ModelState.Remove("Paciente.Nombre");
                ModelState.Remove("Paciente.Apellido");
                ModelState.Remove("Paciente.DNI");
            }

            if (!ModelState.IsValid)
            {
                CargarSelector(turnoNuevo, usuarioId);
                ViewBag.Mensaje = "Error - debe  completar los campos requeridos";


                return View(turnoNuevo);
            }

            //Si no valida;
            if (!_turnoRepository.FechaValida(turnoNuevo.FechaTurno, turnoNuevo.HoraTurno))
            {
                ModelState.AddModelError(nameof(turnoNuevo.FechaTurno),
                    "No se puede elegir una fecha y hora anterior a la actual.");

                CargarSelector(turnoNuevo, usuarioId);

                return View(turnoNuevo);
            }

            // Buscar médico antes de asignar
            Medico? medico = _turnoRepository.AsignarMedico(
                turnoNuevo.EspecialidadId, turnoNuevo.FechaTurno, turnoNuevo.HoraTurno);

            if (medico == null)
            {
                ModelState.AddModelError("", "No hay médicos disponibles para esa especialidad, fecha y hora.");
                CargarSelector(turnoNuevo, usuarioId);
                return View(turnoNuevo);
            }



            try
            {
                if (paciente == null)
                {
                     paciente = new Paciente {
                        Nombre = turnoNuevo.Paciente.Nombre,
                        Apellido = turnoNuevo.Paciente.Apellido,
                        DNI = turnoNuevo.Paciente.DNI,
                        UsuarioId = usuarioId
                    };

                    _turnoRepository.AgregarPaciente(paciente);
                }

                Turno turno = new Turno
                {
                    FechaTurno = turnoNuevo.FechaTurno,
                    EspecialidadId = turnoNuevo.EspecialidadId,
                    HoraTurno = turnoNuevo.HoraTurno,
                    PacienteId = paciente.Id,
                    MedicoId = medico.Id
                };
                
                _turnoRepository.AgregarTurno(turno);

                TempData["Mensaje"] = "Turno creado con exito";
                return RedirectToAction(nameof(Index));

            }
            catch  ( Exception ex)
            {
                ModelState.AddModelError("", "No se pudo crear el turno. Intentá de nuevo.");
                CargarSelector(turnoNuevo, usuarioId);
                return View(turnoNuevo);

            }
        }


        // modificar 
        public ActionResult Edit(int CodigoTurno)
{
    var turnosddbb = _turnoRepository.ObtenerTurnoId(CodigoTurno);
    TurnoAltaVM turnoModificar = new TurnoAltaVM
    {
        CodigoTurno = turnosddbb.CodigoTurno,
        FechaTurno = turnosddbb.FechaTurno,
        EspecialidadId = turnosddbb.EspecialidadId,
        Paciente = new PacienteVM { Nombre = turnosddbb.Paciente.Nombre, Apellido = turnosddbb.Paciente.Apellido, DNI = turnosddbb.Paciente.DNI },
        Especialidades = _turnoRepository.ObtenerEspecialidades()
            .Select(e => new SelectListItem(e.Nombre, e.Id.ToString()))
            .ToList()
    };

    return View(turnoModificar);
}

[HttpPost]
[ValidateAntiForgeryToken]

public ActionResult Edit(int CodigoTurno, TurnoAltaVM turnoNuevo)
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
        var turnoModificar = _turnoRepository.ObtenerTurnoId(CodigoTurno);

        turnoModificar.FechaTurno = turnoNuevo.FechaTurno;
        turnoModificar.EspecialidadId = turnoNuevo.EspecialidadId;

        _turnoRepository.Modificar(turnoModificar);

        TempData["Mensaje"] = "Turno modificado";
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
    var turnosddbb = _turnoRepository.ObtenerTurnoId(CodigoTurno);
    TurnosVM turno = new TurnosVM
    {
        CodigoTurno = turnosddbb.CodigoTurno,
        FechaTurno = turnosddbb.FechaTurno,
        Paciente = new PacienteVM { Nombre = turnosddbb.Paciente.Nombre, Apellido = turnosddbb.Paciente.Apellido, DNI = turnosddbb.Paciente.DNI }
        ,
        Especialidad = turnosddbb.Especialidad.Nombre
    };

    return View(turno);
}


[HttpPost]
[ValidateAntiForgeryToken]
public ActionResult Delete(int CodigoTurno, IFormCollection collection)
{
    try
    {
        var turnosddbb = _turnoRepository.ObtenerTurnoId(CodigoTurno);
        turnosddbb.Borrado = true;
        _turnoRepository.Modificar(turnosddbb);

        return RedirectToAction(nameof(Index));
    }
    catch
    {
        return View();
    }
}
    }   
}