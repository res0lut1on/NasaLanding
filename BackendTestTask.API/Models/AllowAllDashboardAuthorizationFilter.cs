using Hangfire.Dashboard;

namespace BackendTestTask.API.Models
{
    public class AllowAllDashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context) => true;
    }

}
