namespace MVCProject.Models.Views; 

public class GroupViewModel {
    public int Id { get; set; }
    public string Name { get; set; }
    public Dictionary<int, decimal> Balances { get; set; }
    public List<MVCProject.Database.Models.User> Users { get; set; }
    public List<MVCProject.Database.Models.Receipt> Receipts { get; set; }
    public List<MVCProject.Database.Models.Payment> Payments { get; set; }
}