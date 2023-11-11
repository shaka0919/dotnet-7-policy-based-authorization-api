using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using WebApi.Entity;
using WebApi.Model;

namespace WebApi.Authorization;

public class AdminRoleRequirement : IAuthorizationRequirement { }

public class AdminRoleHandler : AuthorizationHandler<AdminRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRoleRequirement requirement)
    {
        if (context.Resource is HttpContext httpContext &&
        httpContext.Items["User"] is User user &&
        user.Role == Role.Admin)
        {
            context.Succeed(requirement);
        }
        return Task.CompletedTask;
    }
}
