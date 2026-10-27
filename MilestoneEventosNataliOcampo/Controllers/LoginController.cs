using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace MilestoneEventosNataliOcampo.web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
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



        //Una acción “Login” para ejecutar la autenticación
        // Una acción que, una vez autenticado, que pueda ver las claims
        //Una acción que permita el cierre de sesión


        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var claims = result?.Principal?.Identities.FirstOrDefault()?.Claims.Select(x => new
            {
                x.Issuer,
                x.OriginalIssuer,
                x.Type,
            });

            return RedirectToAction("Index", "Home");



        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return View("Index");
        }
    }

}
