using Microsoft.AspNetCore.Mvc;
using MVCProject.Database.Contexts;
using Newtonsoft.Json;

namespace MVCProject.Controllers.api; 

[Route("api/needs/")]
public class ApiNeedController : Controller {
    
    [HttpGet]
    [Route("receipt/{id:int}")]
    public string GetForReceipt(int id, [FromServices] ReceiptDatabaseContext context) {
        var needs = context.Needs
            .Where(n => n.receipt_id == id)
            .ToArray();
        
        Response.StatusCode = 200;
        return JsonConvert.SerializeObject(needs);
    }
    
    [HttpGet]
    [Route("group/{id:int}")]
    public string GetForGroup(int id, [FromServices] ReceiptDatabaseContext context) {
        var needs = context.Needs
            .Where(n => n.group_id == id)
            .ToArray();
        
        Response.StatusCode = 200;
        return JsonConvert.SerializeObject(needs);
    }
}