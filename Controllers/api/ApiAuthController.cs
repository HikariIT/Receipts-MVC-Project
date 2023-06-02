using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using MVCProject.Database.Contexts;
using MVCProject.Models;
using MVCProject.Utility;

namespace MVCProject.Controllers.api; 

[Route("api/auth/")]
public class ApiAuthController : Controller {
    
    [HttpPost]
    [Route("login/")]
    public string Login([FromBody] User user, [FromServices] ReceiptDatabaseContext context) {
        var dbUser = context.Users.Where(u => u.mail == user.Mail).ToList().FirstOrDefault();
        
        if (dbUser == null) {
            Response.StatusCode = 404;
            return "User not found";
        }

        if (dbUser.password != user.Password) {
            Response.StatusCode = 401;
            return "Incorrect password";
        }
        
        Response.StatusCode = 200;
        return JwtManager.GenerateJwtToken(dbUser);
    }
    
    [HttpPost]
    [Route("register/")] 
    public string Register([FromBody] User user, [FromServices] ReceiptDatabaseContext context) {
        var dbUser = context.Users.Where(u => u.mail == user.Mail).ToList().FirstOrDefault();
        if (dbUser != null) {
            Response.StatusCode = 409;
            Console.WriteLine("User already exists");
            return "User already exists";
        }
        
        if (user.Username == null) {
            Response.StatusCode = 400;
            Console.WriteLine("Username is null");
            return "Missing fields";
        }
        
        Console.WriteLine("Creating user...");

        context.Users.Add(new Database.Models.User() {
            username = user.Username,
            mail = user.Mail,
            password = user.Password,
            api_token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
        });
        
        context.SaveChanges();

        Response.StatusCode = 200;
        return "User created successfully";
    }
}