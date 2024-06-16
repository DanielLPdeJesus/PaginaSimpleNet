using MiApp.Resources;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyectonext.Models;
using Proyectonext.Models.Data;
using Proyectonext.ViewModels;
using System.ComponentModel.Design;
using System.Net.Mail;
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

    }
}
