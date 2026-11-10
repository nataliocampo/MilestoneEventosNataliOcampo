using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MilestoneEventosNataliOcampo.data.Modelos;
using MilestoneEventosNataliOcampo.data.Repositorios;
using MilestoneEventosNataliOcampo.web.Models;
using System.Diagnostics;
using System.Security.Claims;



namespace MilestoneEventosNataliOcampo.web.Controllers
{
    public class TurnosController : Controller
    {
        private ITurnoRepository _turnoRepository;
      
        public TurnosController(ITurnoRepository turnoRepository)
        {

            _turnoRepository = turnoRepository;
        }
        //mostrar-index- solo del usuario logieado 
        public ActionResult Index()
        {
            //Validacion solo de quien es el turno 
            if (!int.TryParse(User.FindFirstValue("Id"), out int usuarioId))
            {
                return RedirectToAction("Index", "Home");
            }

            var turnoBD = _turnoRepository.ObtenerTurnos()
                .Where(t => !t.Borrado
                    && (t.Paciente.UsuarioId == usuarioId || t.Medico.UsuarioId == usuarioId))
                .ToList();

            List<TurnosVM> Turnos = turnoBD.Select(t => new TurnosVM
            {
                CodigoTurno = t.CodigoTurno,
                FechaTurno = t.FechaTurno,
                HoraTurno = t.HoraTurno,
                Paciente = new PacienteVM { Nombre = t.Paciente.Nombre, Apellido = t.Paciente.Apellido, DNI = t.Paciente.DNI } ,
                Especialidad = t.Especialidad.Nombre,
                Medico = t.Medico.Nombre

            }).ToList();
            return View(Turnos);
        }

        //detail 
        public ActionResult Details(int CodigoTurno)
        {
            if (!int.TryParse(User.FindFirstValue("Id"), out int usuarioId))
            {
                return RedirectToAction("Index", "Home");
            }

            var turnoBD = _turnoRepository.ObtenerTurnoId(CodigoTurno);
            if (turnoBD == null) { return NotFound(); }

            // solo el paciente o medico asignado
            bool esPaciente = turnoBD.Paciente.UsuarioId == usuarioId;
            bool esMedico = turnoBD.Medico.UsuarioId == usuarioId;
            if (!esPaciente && !esMedico) { return Forbid(); }


            var turnoVM = new TurnosVM
            {
                CodigoTurno = turnoBD.CodigoTurno,
                Paciente = new PacienteVM { Nombre = turnoBD.Paciente.Nombre, Apellido = turnoBD.Paciente.Apellido, DNI = turnoBD.Paciente.DNI },
                FechaTurno = turnoBD.FechaTurno,
                HoraTurno = turnoBD.HoraTurno,
                Especialidad = turnoBD.Especialidad.Nombre,
                Medico = turnoBD.Medico.Nombre
            };

           // return View(turnoVM);
            return RedirectToAction(nameof(Details));
        }


        //crear  get 
        public ActionResult Create()
        {
            if (!int.TryParse(User.FindFirstValue("Id"), out int usuarioId))
            {
                return RedirectToAction("Index", "Home");
            }
            TurnoAltaVM turno = new TurnoAltaVM();
            CargarSelector(turno, usuarioId);
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

            //if (paciente != null)
            //{
            //    ModelState.Remove("Paciente.Nombre");
            //    ModelState.Remove("Paciente.Apellido");
            //    ModelState.Remove("Paciente.DNI");
            //}
            if (paciente != null)
            {
                var claves = ModelState.Keys
                    .Where(k => k == "Paciente" || k.StartsWith("Paciente."))
                    .ToList();

                foreach (var k in claves)
                    ModelState.Remove(k);
            }
            else if (turnoNuevo.Paciente == null)
            {
                ModelState.AddModelError("Paciente", "Debe completar los datos del paciente.");
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
                    paciente = new Paciente
                    {
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
            catch 
            {
                ModelState.AddModelError("", "No se pudo crear el turno. Intentá de nuevo.");
                CargarSelector(turnoNuevo, usuarioId);
                return View(turnoNuevo);

            }
        }


        // modificar 
        public ActionResult Edit(int CodigoTurno)
        {

            if (!int.TryParse(User.FindFirstValue("Id"), out int usuarioId))
            {
                return RedirectToAction("Index", "Home");
            }

            var turnoBD = _turnoRepository.ObtenerTurnoId(CodigoTurno);
            if (turnoBD == null) { return NotFound(); }

            // solo el paciente o medico asignado
            bool esPaciente = turnoBD.Paciente.UsuarioId == usuarioId;
            bool esMedico = turnoBD.Medico.UsuarioId == usuarioId;
            if (!esPaciente && !esMedico) { return Forbid(); }

            TurnoAltaVM turnoModificar = new TurnoAltaVM
            {
                CodigoTurno = turnoBD.CodigoTurno,
                FechaTurno = turnoBD.FechaTurno,
                EspecialidadId = turnoBD.EspecialidadId,
                Paciente = new PacienteVM { Nombre = turnoBD.Paciente.Nombre, Apellido = turnoBD.Paciente.Apellido, DNI = turnoBD.Paciente.DNI },
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

            if (!int.TryParse(User.FindFirstValue("Id"), out int usuarioId))
            {
                return RedirectToAction("Index", "Home");
            }

            var turnoBD = _turnoRepository.ObtenerTurnoId(CodigoTurno);
            if (turnoBD == null) { return NotFound(); }

            // solo el paciente o medico asignado
            bool esPaciente = turnoBD.Paciente.UsuarioId == usuarioId;
            bool esMedico = turnoBD.Medico.UsuarioId == usuarioId;
            if (!esPaciente && !esMedico) { return Forbid(); }

            ModelState.Remove("Paciente.Nombre");
            ModelState.Remove("Paciente.Apellido");
            ModelState.Remove("Paciente.DNI");
            if (!ModelState.IsValid)
            {
                CargarSelector(turnoNuevo, usuarioId);
                ViewBag.Mensaje = "Error - debe completar los campos requeridos";
                return View(turnoNuevo);
            }
            
                
            
            try
            {
                turnoBD.FechaTurno = turnoNuevo.FechaTurno;
                turnoBD.EspecialidadId = turnoNuevo.EspecialidadId;

                _turnoRepository.Modificar(turnoBD);

                TempData["Mensaje"] = "Turno modificado";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                CargarSelector(turnoNuevo, usuarioId);
                ModelState.AddModelError("", "No se pudo modificar el turno.");
                return View(turnoNuevo);
            }
        }
        //"eliminar"
        [HttpGet]
        public ActionResult Delete(int CodigoTurno)
        {
            if (!int.TryParse(User.FindFirstValue("Id"), out int usuarioId))
            {
                return RedirectToAction("Index", "Home");
            }

            var turnoBD = _turnoRepository.ObtenerTurnoId(CodigoTurno);
            if (turnoBD == null)
            {
                return NotFound();
            }

            // solo paciente o medico asignado
            bool esPaciente = turnoBD.Paciente.UsuarioId == usuarioId;
            bool esMedico = turnoBD.Medico.UsuarioId == usuarioId;
            if (!esPaciente && !esMedico)
            {
                return Forbid();
            }

            TurnosVM turno = new TurnosVM
            {
                CodigoTurno = turnoBD.CodigoTurno,
                FechaTurno = turnoBD.FechaTurno,
                HoraTurno = turnoBD.HoraTurno,
                Paciente = new PacienteVM
                {
                    Nombre = turnoBD.Paciente.Nombre,
                    Apellido = turnoBD.Paciente.Apellido,
                    DNI = turnoBD.Paciente.DNI
                },
                Especialidad = turnoBD.Especialidad.Nombre,
                Medico = turnoBD.Medico.Nombre
            };

            return View(turno);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int CodigoTurno, IFormCollection collection)
        {
            if (!int.TryParse(User.FindFirstValue("Id"), out int usuarioId))
            {
                return RedirectToAction("Index", "Home");
            }

            var turnoBD = _turnoRepository.ObtenerTurnoId(CodigoTurno);
            if (turnoBD == null)
            {
                return NotFound();
            }

            // solo paciente o medico asignado
            bool esPaciente = turnoBD.Paciente.UsuarioId == usuarioId;
            bool esMedico = turnoBD.Medico.UsuarioId == usuarioId;
            if (!esPaciente && !esMedico)
            {
                return Forbid();
            }

            try
            {
                turnoBD.Borrado = true;
                _turnoRepository.Modificar(turnoBD);

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}