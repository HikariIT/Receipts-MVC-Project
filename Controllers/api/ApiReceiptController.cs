using Microsoft.AspNetCore.Mvc;
using MVCProject.Database.Contexts;
using MVCProject.Models;
using Newtonsoft.Json;

namespace MVCProject.Controllers.api; 

[Route("api/receipts/")]
public class ApiReceiptController: Controller {
    
    [HttpPost]
    [Route("add/")]
    public string Add([FromBody] Receipt receipt, [FromServices] ReceiptDatabaseContext context) {
        var dbReceipt = new MVCProject.Database.Models.Receipt() {
            group_id = receipt.GroupId,
            paying_id = receipt.PayingId,
            name = receipt.Name,
            date = receipt.Date,
            value = receipt.Value
        };
        
        var createdReceipt = context.Receipts.Add(dbReceipt);
        context.SaveChanges();

        var dbPayment = new MVCProject.Database.Models.Payment() {
            group_id = receipt.GroupId,
            user_id = receipt.PayingId,
            value = receipt.Value,
            date = receipt.Date,
            targeted = false,
            receipt_id = createdReceipt.Entity.receipt_id ?? -1,
            targeted_user_id = null
        };
        context.Payments.Add(dbPayment);

        var no_users = receipt.Needs.Count;
        var change = decimal.ToInt32(receipt.SharedValue * 100) % no_users; // In cents / grosze etc.
        var value = decimal.ToInt32(receipt.SharedValue * 100 - change) / no_users;
        var userList = receipt.Needs.Keys.ToList();

        var rnd = new Random();
        var randomUsers = userList.OrderBy(x => rnd.Next()).Take(change).ToList(); // Users to add 1 cent / gr to
        foreach (var randomUser in randomUsers) {
            receipt.Needs[randomUser] +=  0.01m;
        }

        foreach (var user in receipt.Needs.Keys.ToList()) {
            receipt.Needs[user] += Convert.ToDecimal(value) / 100m;
        }

        var dbNeeds = new List<MVCProject.Database.Models.Need>();
        foreach(var need in receipt.Needs) {
            dbNeeds.Add(new MVCProject.Database.Models.Need {
                receipt_id = createdReceipt.Entity.receipt_id ?? -1,
                group_id = receipt.GroupId,
                user_id = need.Key,
                value = need.Value
            });
        }

        context.Needs.AddRange(dbNeeds);
        context.SaveChanges();
        
        Response.StatusCode = 200;
        return "Receipt added successfully";
    }

    [HttpGet]
    [Route("{id:int}/")]
    public string Get(int id, [FromServices] ReceiptDatabaseContext context) {
        var receipt = context.Receipts
            .FirstOrDefault(r => r.receipt_id == id);

        if (receipt == null) {
            Response.StatusCode = 404;
            return "Receipt not found";
        }
        
        Response.StatusCode = 200;
        return JsonConvert.SerializeObject(receipt);
    }

    [HttpGet]
    [Route("group/{id:int}/")]
    public string GetByGroup(int id, [FromServices] ReceiptDatabaseContext context) {
        var receipts = context.Receipts
            .Where(r => r.group_id == id)
            .ToArray();
        
        Response.StatusCode = 200;
        return JsonConvert.SerializeObject(receipts);
    }
}