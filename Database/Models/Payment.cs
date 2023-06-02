using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCProject.Database.Models; 

[Table("payments")]
public class Payment {
    [Key]
    public int? payment_id { get; set; }
    public int user_id { get; set; }
    public int group_id { get; set; }
    public int? receipt_id { get; set; }
    public string date { get; set; }
    public decimal value { get; set; }
    public bool targeted { get; set; }
    public int? targeted_user_id { get; set; }
}