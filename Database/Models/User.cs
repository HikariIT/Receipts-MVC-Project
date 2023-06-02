using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVCProject.Database.Models; 

[Table("users")]
public class User {
    [Key]
    public int? user_id { get; set; }
    public string username { get; set; }
    public string mail { get; set; }
    public string api_token { get; set; }
    public string password { get; set; }
}