using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MVCProject.Database.Models; 

[Table("participation")]
[PrimaryKey(nameof(user_id), nameof(group_id))]
public class Participation {
    public int user_id { get; set; }
    public int group_id { get; set; }
}