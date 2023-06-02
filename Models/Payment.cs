namespace MVCProject.Models; 

public class Payment {
    public int UserId { get; set; }
    public int GroupId { get; set; }
    public string Date { get; set; }
    public decimal Value { get; set; }
    public int TargetedUserId { get; set; }
}