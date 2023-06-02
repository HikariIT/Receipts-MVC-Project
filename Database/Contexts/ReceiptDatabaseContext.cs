using Microsoft.EntityFrameworkCore;
using MVCProject.Database.Models;

namespace MVCProject.Database.Contexts; 

public class ReceiptDatabaseContext: DbContext {
    
    public DbSet<User> Users { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Need> Needs { get; set; }
    public DbSet<Participation> Participation { get; set; }
    public DbSet<Payment> Payments { get; set; }
    
    public ReceiptDatabaseContext(DbContextOptions<ReceiptDatabaseContext> options): base(options) { }
}