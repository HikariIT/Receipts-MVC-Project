using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCProject.Database.Models; 

[Table("groups")]
public class Group {
    [Key]
    public int? group_id { get; set; }
    public int no_users { get; set; }
    public string name { get; set; }
}