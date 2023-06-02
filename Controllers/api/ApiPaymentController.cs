using Microsoft.AspNetCore.Mvc;
using MVCProject.Database.Contexts;
using MVCProject.Models;
using Newtonsoft.Json;

namespace MVCProject.Controllers.api; 

[Route("api/payments/")]
public class ApiPaymentController : Controller {

    [HttpGet]
    [Route("group/{id:int}")]
    public string GetByGroup(int id, [FromServices] ReceiptDatabaseContext context) {
        var payments = context.Payments
            .Where(p => p.group_id == id)
            .ToArray();
        
        Response.StatusCode = 200;
        return JsonConvert.SerializeObject(payments);
    }

    [HttpPost]
    [Route("add/")]
    public string Add([FromBody] Payment payment, [FromServices] ReceiptDatabaseContext context) {
        var dbPayment = new MVCProject.Database.Models.Payment() {
            group_id = payment.GroupId,
            user_id = payment.UserId,
            value = payment.Value,
            date = payment.Date,
            targeted = true,
            targeted_user_id = payment.TargetedUserId
        };
        
        context.Payments.Add(dbPayment);
        context.SaveChanges();
        
        Response.StatusCode = 200;
        return "Payment added successfully";
    }
}