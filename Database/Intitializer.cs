using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using MVCProject.Database.Contexts;
using MVCProject.Database.Models;

namespace MVCProject.Database; 

public static class DatabaseInitializer {
    public static void Initialize(IServiceProvider serviceProvider) {
        using var context = new ReceiptDatabaseContext(serviceProvider.GetRequiredService<DbContextOptions<ReceiptDatabaseContext>>());
        context.Database.EnsureCreated();

        if (context.Users.Any())
            return;

        context.Users.Add(new User {
            username = "Admin",
            mail = "admin@receipts.service.com",
            password = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes("root"))),
            api_token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32))
        }); 
        
        context.SaveChanges();
    }
}