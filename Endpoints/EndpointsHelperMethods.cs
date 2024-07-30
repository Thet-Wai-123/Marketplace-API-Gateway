using System.Security.Claims;

namespace MarketPlace_API_Gateway.Endpoints
{
    public static class EndpointsHelperMethods
    {
        public static void AddUserIdToPostedBy(HttpContext httpContext, dynamic taskInfo)
        {
            var userId = Int32.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            taskInfo.PostedBy = userId;
        }
    }
}
