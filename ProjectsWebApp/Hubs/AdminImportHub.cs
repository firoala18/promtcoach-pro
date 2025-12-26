using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ProjectsWebApp.Hubs
{
    [Authorize(Roles = "Admin,SuperAdmin,Dozent")] 
    public class AdminImportHub : Hub
    {
    }
}
