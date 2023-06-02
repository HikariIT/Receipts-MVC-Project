using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using MVCProject.Models;
using MVCProject.Utility;

namespace MVCProject.Controllers; 

public class AuthController : Controller {

    [HttpGet]
    public IActionResult Login() {
        return View();
    }

    [HttpPost]
    public IActionResult LoginForm(User user) {
        user.Password = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(user.Password)));
        
        var response = ApiCallManager.PostHeaderless("/auth/login/", user).Result;

        if (response.StatusCode == HttpStatusCode.OK) {
            var token = response.Content.ReadAsStringAsync().Result;
            HttpContext.Session.SetString("user", token);
        }

        return response.StatusCode == HttpStatusCode.OK ? 
            RedirectToAction("Dashboard", "Home") : 
            RedirectToAction("Login", "Auth");
    }
    
    [HttpGet]
    public IActionResult Register() {
        return View();
    }

    [HttpPost]
    public IActionResult RegisterForm(User user) {
        user.Password = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(user.Password)));
        var response = ApiCallManager.PostHeaderless("/auth/register/", user).Result;

        return response.StatusCode == HttpStatusCode.OK ? 
            RedirectToAction("Login", "Auth") : 
            RedirectToAction("Register", "Auth");
    }
    
    [HttpGet]
    public IActionResult Logout() {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}