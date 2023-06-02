using Microsoft.AspNetCore.Mvc;
using MVCProject.Database.Contexts;
using MVCProject.Database.Models;
using MVCProject.Models;
using Newtonsoft.Json;
using Group = MVCProject.Models.Group;

namespace MVCProject.Controllers.api; 

[Route("api/groups/")]
public class ApiGroupController : Controller {
    
    [HttpGet]
    [Route("users/{id:int}/")]
    public string GetByUser(int id, [FromServices] ReceiptDatabaseContext context) {
        var groups = context.Participation
            .Where(p => p.user_id == id)
            .Join(context.Groups, p => p.group_id, g => g.group_id, (p, g) => g)
            .ToArray();
        
        Response.StatusCode = 200;
        Console.WriteLine(JsonConvert.SerializeObject(groups));
        return JsonConvert.SerializeObject(groups);
    }

    [HttpPost]
    [Route("create/")]
    public string Create([FromServices] ReceiptDatabaseContext context, [FromBody] Group parameters) {
        var dbGroup = context.Groups.Add(new Database.Models.Group() {
            name = parameters.Name,
            no_users = 1
        });

        context.SaveChanges();
        
        var dbParticipation = context.Participation.Add(new Participation() {
            group_id = dbGroup.Entity.group_id ?? -1,
            user_id = parameters.User
        });
        
        context.SaveChanges();
        Response.StatusCode = 200;
        return "Group created successfully";
    }

    [HttpPost]
    [Route("add/{id:int}/")]
    public string Add(int id, [FromServices] ReceiptDatabaseContext context, [FromBody] string username) {
        
        var user = context.Users.FirstOrDefault(u => u.username == username);
        if (user == null) {
            Response.StatusCode = 404;
            return "User not found";
        }
        
        Console.WriteLine(user.user_id);
        var participation = context.Participation
            .Where(p => p.group_id == id && p.user_id == user.user_id)
            .ToList()
            .FirstOrDefault();
        
        if (participation != null) {
            Response.StatusCode = 409;
            return "User already in group";
        }
        
        context.Participation.Add(new Participation() {
            group_id = id,
            user_id = user.user_id ?? -1
        });
        
        context.SaveChanges();
        Response.StatusCode = 200;
        return "User joined group successfully";
    }

    [HttpGet]
    [Route("participants/{id:int}")]
    public string GetUsers(int id, [FromServices] ReceiptDatabaseContext context) {
        var users = context.Participation
            .Where(p => p.group_id == id)
            .Join(context.Users, p => p.user_id, u => u.user_id, (p, u) => u)
            .ToList();
        
        // Set passwords to ""
        users.ForEach(u => {
            u.password = u.api_token = "";
        });

        Response.StatusCode = 200;
        return JsonConvert.SerializeObject(users.ToArray());
    }
}