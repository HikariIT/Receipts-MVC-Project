using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCProject.Models.Views;
using MVCProject.Utility;
using Newtonsoft.Json;
using Group = MVCProject.Models.Group;
using Payment = MVCProject.Models.Payment;
using Receipt = MVCProject.Models.Receipt;

namespace MVCProject.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index() {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    /// <summary>
    /// This method displays the Dashboard, where user can access groups they are part of or create a new one.
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Dashboard(DashboardViewModel model) {
        
        var token = HttpContext.Session.GetString("user");
        var data = JwtManager.ExtractPayload(token!);

        var response = ApiCallManager.Get("/groups/users/" + data.user, data.api_key, data.user).Result;
        var groups = JsonConvert.DeserializeObject<Database.Models.Group[]>(response.Content.ReadAsStringAsync().Result);
        
        model.Groups = groups;
        return View(model);
    }

    /// <summary>
    /// This method displays control panel for a specific group.
    /// </summary>
    /// <param name="id">Group ID</param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Group(int id) {
        var token = HttpContext.Session.GetString("user");
        var data = JwtManager.ExtractPayload(token!);
        
        // This verification is to check if user is accessing a group he is not part of
        var response = ApiCallManager.Get("/groups/users/" + data.user + "/", data.api_key, data.user).Result;
        var groups = JsonConvert.DeserializeObject<Database.Models.Group[]>(response.Content.ReadAsStringAsync().Result);

        if (groups.All(g => g.group_id != id)) {    
            return RedirectToAction("Dashboard", "Home");
        }

        var groupData = groups.ToList().Find(g => g.group_id == id);
        
        var participants = ApiCallManager.Get("/groups/participants/" + id + "/", data.api_key, data.user).Result;
        var users = JsonConvert.DeserializeObject<MVCProject.Database.Models.User[]>(participants.Content.ReadAsStringAsync().Result);

        var receipts = ApiCallManager.Get("/receipts/group/" + id + "/", data.api_key, data.user).Result;
        var receiptsData = JsonConvert.DeserializeObject<Database.Models.Receipt[]>(receipts.Content.ReadAsStringAsync().Result);
        
        var payments = ApiCallManager.Get("/payments/group/" + id + "/", data.api_key, data.user).Result;
        var paymentsData = JsonConvert.DeserializeObject<Database.Models.Payment[]>(payments.Content.ReadAsStringAsync().Result);

        var userPaymentDict = new Dictionary<int, decimal>();
        
        foreach (var user in users) {
            userPaymentDict.Add(user.user_id ?? -1, 0);
        }
        foreach (var payment in paymentsData) {
            if (!payment.targeted)
                userPaymentDict[payment.user_id] -= payment.value;
            else {
                userPaymentDict[payment.user_id] -= payment.value;
                userPaymentDict[payment.targeted_user_id ?? -1] += payment.value;
            }
        }
        
        var needs = ApiCallManager.Get("/needs/group/" + id + "/", data.api_key, data.user).Result;
        var needsData = JsonConvert.DeserializeObject<Database.Models.Need[]>(needs.Content.ReadAsStringAsync().Result);


        foreach (var need in needsData) {
            userPaymentDict[need.user_id] += need.value;
        }

        Console.WriteLine(JsonConvert.SerializeObject(userPaymentDict));
        
        var model = new GroupViewModel() {
            Id = groupData.group_id ?? -1,
            Name = groupData.name,
            Users = users.ToList(),
            Receipts = receiptsData.ToList(),
            Payments = paymentsData.ToList(),
            Balances = userPaymentDict
        };

        return View(model);
    }

    /// <summary>
    /// This method displays a view for a specific receipt.
    /// </summary>
    /// <param name="id">Receipt ID</param>
    /// <returns></returns>
    [HttpGet]
    public IActionResult Receipt(int id) {
        var token = HttpContext.Session.GetString("user");
        var data = JwtManager.ExtractPayload(token!);
        
        var response = ApiCallManager.Get("/receipts/" + id + "/", data.api_key, data.user).Result;
        var receipt = JsonConvert.DeserializeObject<Database.Models.Receipt>(response.Content.ReadAsStringAsync().Result);
        
        var needs = ApiCallManager.Get("/needs/receipt/" + id + "/", data.api_key, data.user).Result;
        var needsData = JsonConvert.DeserializeObject<Database.Models.Need[]>(needs.Content.ReadAsStringAsync().Result);
        
        var users = ApiCallManager.Get("/groups/participants/" + receipt.group_id + "/", data.api_key, data.user).Result;
        var usersData = JsonConvert.DeserializeObject<Database.Models.User[]>(users.Content.ReadAsStringAsync().Result);
        
        var groups = ApiCallManager.Get("/groups/users/" + data.user + "/", data.api_key, data.user).Result;
        var groupsData = JsonConvert.DeserializeObject<Database.Models.Group[]>(groups.Content.ReadAsStringAsync().Result);

        if (groupsData.All(g => g.group_id != receipt.group_id)) {    
            return RedirectToAction("Dashboard", "Home");
        }

        var group = groupsData.ToList().Find(g => g.group_id == receipt.group_id);

        var model = new ReceiptViewModel() {
            Id = receipt.receipt_id ?? -1,
            Name = receipt.name,
            Value = receipt.value,
            Date = receipt.date,
            Needs = needsData.ToList(),
            Users = usersData.ToList(),
            GroupName = group.name
        };
        
        return View(model);
    }

    /// <summary>
    /// This method displays a view for Admin panel, where the admin can manage users.
    /// </summary>
    [HttpGet]
    public IActionResult Admin() {
        
        var token = HttpContext.Session.GetString("user");
        var data = JwtManager.ExtractPayload(token!);
        
        var users = ApiCallManager.Get("/users/get", data.api_key, data.user).Result;
        var usersData = JsonConvert.DeserializeObject<Database.Models.User[]>(users.Content.ReadAsStringAsync().Result);
        
        var model = new AdminViewModel() {
            Users = usersData.ToList()
        };

        return View(model);
    }
    
    [HttpGet]
    public IActionResult DeleteUser(int id) {
        var token = HttpContext.Session.GetString("user");
        var data = JwtManager.ExtractPayload(token!);
        
        var response = ApiCallManager.Delete("/users/delete/" + id, data.api_key, data.user).Result;
        Console.WriteLine("RES: " + response.Content.ReadAsStringAsync().Result);
        return RedirectToAction("Admin", "Home");
    }
    
    [HttpPost]
    public IActionResult CreateGroup([FromForm] string name) {
        var token = HttpContext.Session.GetString("user");
        var data = JwtManager.ExtractPayload(token!);
        
        var response = ApiCallManager.Post("/groups/create/", data.api_key, data.user, 
            new Group { Name = name, User = int.Parse(data.user)}).Result;
        Console.WriteLine("RES: " + response.Content.ReadAsStringAsync().Result);
        return RedirectToAction("Dashboard", "Home");
    }
    
    [HttpPost]
    public IActionResult AddToGroup([FromForm] string name, [FromForm] int groupId) {
        var token = HttpContext.Session.GetString("user");
        var data = JwtManager.ExtractPayload(token!);
        
        var response = ApiCallManager.Post("/groups/add/" + groupId, data.api_key, data.user, name).Result;
        Console.WriteLine("RES: " + response.Content.ReadAsStringAsync().Result);
        return RedirectToAction("Group", "Home", new {
            id = groupId
        });
    }

    [HttpPost]
    public IActionResult AddReceipt(IFormCollection data) {

        Console.WriteLine(data);
        Console.WriteLine("SER: " + JsonConvert.SerializeObject(data));

        var users = data["users"][0].Split(", ").Select(u => int.Parse(u));
        var needs = users
            .Select(u => new KeyValuePair<int, decimal>(u, decimal.Parse(data["need_" + u][0].Replace(".", ","))))
            .ToDictionary(k => k.Key, v => v.Value);

        var receiptCreationBody = new Receipt {
            Name = data["name"],
            Date = data["date"],
            Value = decimal.Parse(data["value"][0].Replace(".", ",")),
            SharedValue = decimal.Parse(data["shared"][0].Replace(".", ",")),
            GroupId = int.Parse(data["groupId"]),
            PayingId = int.Parse(data["payer"]),
            Needs = needs
        };
        
        Console.WriteLine(JsonConvert.SerializeObject(receiptCreationBody, Formatting.Indented));
        
        var token = HttpContext.Session.GetString("user");
        var jwtData = JwtManager.ExtractPayload(token!);
        
        var response = ApiCallManager.Post("/receipts/add/", jwtData.api_key, jwtData.user, receiptCreationBody).Result;
        
        return RedirectToAction("Group", "Home", new {
            id = receiptCreationBody.GroupId
        });
    }

    [HttpPost]
    public IActionResult AddPayment(IFormCollection data) {
        
        var token = HttpContext.Session.GetString("user");
        var jwtData = JwtManager.ExtractPayload(token!);
        
        var paymentCreationBody = new Payment {
            Date = data["date"],
            Value = decimal.Parse(data["value"][0].Replace(".", ",")),
            GroupId = int.Parse(data["groupId"]),
            UserId = int.Parse(data["payer"]),
            TargetedUserId = int.Parse(data["target"])
        };
        
        var response = ApiCallManager.Post("/payments/add/", jwtData.api_key, jwtData.user, paymentCreationBody).Result;

        return RedirectToAction("Group", "Home", new {
            id = paymentCreationBody.GroupId
        });
    }
}


