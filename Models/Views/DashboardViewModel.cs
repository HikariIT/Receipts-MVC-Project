using MVCProject.Database.Models;

namespace MVCProject.Models.Views; 

public class DashboardViewModel {
    public Database.Models.Group[] Groups { get; set; }
}