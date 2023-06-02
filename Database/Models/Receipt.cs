    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCProject.Database.Models; 

[Table("receipts")]
public class Receipt {
    [Key]
    public int? receipt_id { get; set; }
    public int group_id { get; set; }
    public int paying_id { get; set; }
    public string name { get; set; }
    public string date { get; set; }
    public decimal value { get; set; }
}