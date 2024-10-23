using Microsoft.AspNetCore.Mvc;
using energy_knowledge.Data;
using energy_knowledge.Models;
using Microsoft.EntityFrameworkCore;
using energy_knowledge.ViewModels;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace energy_knowledge.Controllers
{
    public class AccesoController : Controller
    {
        private readonly AppDBContext _appDbContext;
        public AccesoController(AppDBContext appDBContext) 
        {
            _appDbContext = appDBContext;
        }

        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Registrarse(UsuarioVM modelo)
        {
            if (modelo.Contrasenia != modelo.ConfirmarContrasenia)
            {
                ViewData["Mensaje"] = "Las contraseñas no coinciden";
                return View();
            }

            Usuario usuario = new Usuario()
            {
                Nombre = modelo.Nombre,
                Apellidos = modelo.Apellidos,
                Correo = modelo.Correo,
                Contrasenia = modelo.Contrasenia,
            };

            await _appDbContext.Usuarios.AddAsync(usuario);
            await _appDbContext.SaveChangesAsync();

            if(usuario.IdUsuario != 0) { return RedirectToAction("Login","Acceso");}
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if(User.Identity!.IsAuthenticated) { return RedirectToAction("Index", "Home"); }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM modelo)
        {
            Usuario? usuario_existe = await _appDbContext.Usuarios.Where(u =>
                                    u.Correo == modelo.Correo &&
                                    u.Contrasenia == modelo.Contrasenia
                                    ).FirstOrDefaultAsync();

            if (usuario_existe == null)
            {
                ViewData["Mensaje"] = "No se encontraron coincidencias";
                return View();
            }

            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, usuario_existe.Nombre)

            };
            
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims,CookieAuthenticationDefaults.AuthenticationScheme);
            AuthenticationProperties propierties = new AuthenticationProperties()
            {
                AllowRefresh = true,
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                propierties
                );

            return RedirectToAction("Index","Home");
        }
    }
}
