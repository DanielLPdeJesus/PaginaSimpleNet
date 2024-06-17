using MiApp.Resources;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyectonext.Models;
using Proyectonext.Models.Data;
using Proyectonext.ViewModels;
using System.ComponentModel.Design;
using System.Net.Mail;
using System.Security.Claims;
namespace Proyectonext.Controllers
{
    public class AccesoController : Controller
    { 
        private readonly appDbContext _appDbContext;
        public AccesoController(appDbContext appDbContext)
        {
            _appDbContext = appDbContext;

        }
        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Registrarse( UsersVM modelito)
        {
           if (modelito.Password != modelito.RptPasssword) { ViewData["Mensaje"] = "las contraseñas no coinciden";
            return View();
            }
            Users? userFind = await _appDbContext.Users.Where(u => u.UserName == modelito.UserName).FirstOrDefaultAsync();
            if (userFind == null)
            {
                modelito.Password = Tools.Encriptar(modelito.Password);
                Users users = new Users() { UserName = modelito.UserName, Password = modelito.Password, Mail = modelito.Mail, Active = true };
                await _appDbContext.Users.AddAsync(users);
                await _appDbContext.SaveChangesAsync();
                if (users.IdUser == 0)
                {
                    ViewData["Mensaje"] = "Error Fatal";
                }
                else { return RedirectToAction("Login", "Acceso"); }
            }
            else { ViewData["Mensaje"] = "El usuario ya existe"; }
            return View();
            
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(Users modelo)
        {
            // Buscar el usuario en la base de datos
            Users? userFind = await _appDbContext.Users
                .Where(u => u.Mail == modelo.Mail && u.Password == modelo.Password && u.Active == true)
                .FirstOrDefaultAsync();

            // Verificar si se encontró el usuario
            if (userFind != null)
            {
                // Crear los claims para el usuario autenticado
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, userFind.Mail),
            new Claim(ClaimTypes.NameIdentifier, userFind.Password.ToString())
        };
                // Crear la identidad del usuario y definir las propiedades de autenticación
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                AuthenticationProperties properties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true
                };

                // Autenticar al usuario
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);


                // Redirigir al usuario a la página principal después de la autenticación exitosa
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // Mostrar mensaje de error si el usuario no existe
                ViewData["Mensaje"] = "El usuario no Existe";
                return View();
            }
        }


    }
}
