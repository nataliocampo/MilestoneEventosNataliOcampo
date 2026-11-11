using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using MilestoneEventosNataliOcampo.data.Repositorios;
using System.Security.Claims;

namespace MilestoneEventosNataliOcampo.web.Controllers
{
    public class LoginController : Controller
    {
        private IUsuarioRepository _usuarioRepository;

        public LoginController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            return View();
        
        }
       

        public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme,
                new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("GoogleResponse")
                }
               );
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            //Si no esta en tabla

            var email = result.Principal.FindFirstValue(ClaimTypes.Email);
            var nombre = result.Principal.FindFirstValue(ClaimTypes.Name);
            var user = _usuarioRepository.ObtenerOCrearUsuario(email, nombre);

            List<String> listaPermisos = _usuarioRepository.ObtenerPermisos(email);


            var claimsPer = new List<Claim>(result.Principal.Claims)
            {
                new Claim("Id", user.Id.ToString())


            };

            foreach (var permiso in listaPermisos)
            {
                claimsPer.Add(new Claim("Permiso", permiso));
            }

            var identity = new ClaimsIdentity(claimsPer, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);


            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal);

            return RedirectToAction("Index", "Home");
            
        }

        //Una acción “Login” para ejecutar la autenticación
        // Una acción que, una vez autenticado, que pueda ver las claims
        //Una acción que permita el cierre de sesión

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Login");
        }
    }

}
