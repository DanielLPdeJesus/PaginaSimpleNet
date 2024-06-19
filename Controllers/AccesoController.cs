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

//Declara el namespace y la clase AccesoController, que hereda de Controller. Define una variable privada para el contexto de la base de datos.
namespace Proyectonext.Controllers
{
    public class AccesoController : Controller
    {
        //Constructor de la clase AccesoController, que inicializa el contexto de la base de datos.
        private readonly appDbContext _appDbContext;
        public AccesoController(appDbContext appDbContext)
        {
            _appDbContext = appDbContext;

        }
        //Método Registrarse que responde a solicitudes GET y devuelve la vista para el registro de usuario.
        [HttpGet]
        public IActionResult Registrarse()
        {
            return View();
        }

        //Método Registrarse que responde a solicitudes POST. Comprueba si las contraseñas coinciden, y si no, muestra un mensaje de error.
        [HttpPost]
        public async Task<IActionResult> Registrarse( UsersVM modelito)
        {
           if (modelito.Password != modelito.RptPasssword) { ViewData["Mensaje"] = "las contraseñas no coinciden";
            return View();
            }
            //Busca en la base de datos un usuario con el mismo nombre de usuario que el proporcionado en el modelo.
            Users? userFind = await _appDbContext.Users.Where(u => u.UserName == modelito.UserName).FirstOrDefaultAsync();
            //Si el usuario no existe, encripta la contraseña y crea un nuevo usuario con los datos del modelo. Luego, lo guarda en la base de datos.
            if (userFind == null)
            {
                modelito.Password = Tools.Encriptar(modelito.Password);
                Users users = new Users() { UserName = modelito.UserName, Password = modelito.Password, Mail = modelito.Mail, Active = true };
                await _appDbContext.Users.AddAsync(users);
                await _appDbContext.SaveChangesAsync();
                //Verifica si el usuario fue guardado correctamente. Si hay un error, muestra un mensaje de error.
                if (users.IdUser == 0)
                {
                    ViewData["Mensaje"] = "Error Fatal";
                }
                else { return RedirectToAction("Login", "Acceso"); }
            }
            else { ViewData["Mensaje"] = "El usuario ya existe"; }
            return View();
            
        }

        //Método Login que responde a solicitudes GET y devuelve la vista para el registro de usuario.
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]  
        public async Task<IActionResult> Login(Users modelo)
        {
            // Encripta la contraseña del modelo
            modelo.Password = Tools.Encriptar(modelo.Password);
            // Busca un usuario en la base de datos con el correo y contraseña proporcionados, que esté activo
            Users? userFind = await _appDbContext.Users.Where(u => u.Mail == modelo.Mail && u.Password == modelo.Password && u.Active == true).FirstOrDefaultAsync();
            // Si el usuario existe, crea una lista de claims para el usuario
            if (userFind != null)
            {
                var claims = new List<Claim>{
                new Claim(ClaimTypes.Email, userFind.Mail),
                new Claim(ClaimTypes.NameIdentifier, userFind.IdUser.ToString())};

                // Crea una identidad de claims con el esquema de autenticación de cookies
                ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                // Define las propiedades de autenticación
                AuthenticationProperties properties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = false
                };
                // Realiza el login con el esquema de autenticación de cookies
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), properties);
                // Redirige al usuario a la página de inicio
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
