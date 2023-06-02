using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MVCProject.Database.Models; 

[Table("needs")]
[PrimaryKey(nameof(user_id), nameof(receipt_id))]
public class Need {
    public int user_id { get; set; }
    public int group_id { get; set; }
    public int receipt_id { get; set; }
    public decimal value { get; set; }
}