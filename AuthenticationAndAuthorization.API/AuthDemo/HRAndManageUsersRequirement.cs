using Microsoft.AspNetCore.Authorization;

namespace AuthenticationAndAuthorization.API.AuthDemo
{
    public class HRAndManageUsersRequirement : IAuthorizationRequirement { }

    public class HRAndManageUsersHandler : AuthorizationHandler<HRAndManageUsersRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HRAndManageUsersRequirement requirement)
        {
            var hasHr = context.User.HasClaim("Department", "HR");
            var hasPermission = context.User.HasClaim("Permission", "ManageUsers");

            if (hasHr && hasPermission)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}