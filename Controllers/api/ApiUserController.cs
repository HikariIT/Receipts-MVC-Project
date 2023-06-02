using Microsoft.AspNetCore.Mvc;
using MVCProject.Database.Contexts;
using Newtonsoft.Json;

namespace MVCProject.Controllers.api; 

[Route("api/users/")]
public class ApiUserController : Controller {
    
    [HttpGet]
    [Route("{id:int}/")]
    public string Get(int id, [FromServices] ReceiptDatabaseContext context) {
        var dbUser = context.Users.Where(u => u.user_id == id).ToList().FirstOrDefault();
        if (dbUser == null) {
            Response.StatusCode = 404;
            return "User not found";
        }
        
        Response.StatusCode = 200;
        dbUser.password = "";
        return JsonConvert.SerializeObject(dbUser);
    }
    
    [HttpGet]
    [Route("get/")]
    public string GetAll([FromServices] ReceiptDatabaseContext context) {
        var dbUsers = context.Users.ToArray();
        foreach(var user in dbUsers) {
            user.password = "";
        }
        
        Response.StatusCode = 200;
        return JsonConvert.SerializeObject(dbUsers);
    }
    
    [HttpDelete]
    [Route("delete/{id:int}/")]
    public string Delete(int id, [FromServices] ReceiptDatabaseContext context) {
        var dbUser = context.Users.Where(u => u.user_id == id).ToList().FirstOrDefault();
        if (dbUser == null) {
            Response.StatusCode = 404;
            return "User not found";
        }
        
        context.Users.Remove(dbUser);
        context.SaveChanges();
        
        Response.StatusCode = 200;
        return "User deleted successfully";
    }
}