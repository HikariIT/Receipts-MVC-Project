namespace MVCProject.Models.Views; 

public class ReceiptViewModel {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Date { get; set; }
    public string GroupName { get; set; }
    public decimal Value { get; set; }
    public List<MVCProject.Database.Models.Need> Needs { get; set; }
    public List<MVCProject.Database.Models.User> Users { get; set; }
}