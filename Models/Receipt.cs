namespace MVCProject.Models; 

public class Receipt {
    public string Name { get; set; }
    public string Date { get; set; }
    public decimal Value { get; set; }
    public Dictionary<int, decimal> Needs { get; set; }
    public decimal SharedValue { get; set; }
    public int PayingId { get; set; }
    public int GroupId { get; set; }
}