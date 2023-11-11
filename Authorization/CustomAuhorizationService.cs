using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using WebApi.Service;

namespace WebApi.Authorization;

public class CustomAuhorizationService : IAuthorizationService
{
    private readonly AuthorizationOptions _options;
    private readonly IAuthorizationHandlerProvider _handlers;
    private readonly IAuthorizationPolicyProvider _policyProvider;
    private readonly ILogger<DefaultAuthorizationService> _logger;
    private readonly IAuthorizationEvaluator _evaluator;
    private readonly IAuthorizationHandlerContextFactory _contextFactory;
    private readonly IUserService _userService;

    public CustomAuhorizationService(IAuthorizationPolicyProvider policyProvider, IAuthorizationHandlerProvider handlers, ILogger<DefaultAuthorizationService> logger, IAuthorizationHandlerContextFactory contextFactory, IAuthorizationEvaluator evaluator, IOptions<AuthorizationOptions> options, IUserService userService)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(handlers, nameof(handlers));
        ArgumentNullException.ThrowIfNull(policyProvider, nameof(policyProvider));
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        ArgumentNullException.ThrowIfNull(evaluator, nameof(evaluator));
        ArgumentNullException.ThrowIfNull(contextFactory, nameof(contextFactory));
        ArgumentNullException.ThrowIfNull(userService, nameof(userService));

        _options = options.Value;
        _handlers = handlers;
        _policyProvider = policyProvider;
        _logger = logger;
        _evaluator = evaluator;
        _contextFactory = contextFactory;
        _userService = userService;
    }

    public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, IEnumerable<IAuthorizationRequirement> requirements)
    {
        ArgumentNullException.ThrowIfNull(requirements, nameof(requirements));

        if (user.HasClaim(x => x.Type == ClaimTypes.NameIdentifier))
        {
            var parseResult = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out int userId);

            if (parseResult && resource is HttpContext context)
            {
                var userEntity = _userService.GetById(userId);
                context.Items["User"] = userEntity;
            }
        }

        var authContext = _contextFactory.CreateContext(requirements, user, resource);
        var handlers = await _handlers.GetHandlersAsync(authContext).ConfigureAwait(false);
        foreach (var handler in handlers)
        {
            await handler.HandleAsync(authContext).ConfigureAwait(false);
            if (!_options.InvokeHandlersAfterFailure && authContext.HasFailed)
            {
                break;
            }
        }

        var result = _evaluator.Evaluate(authContext);

        return result;
    }

    public async Task<AuthorizationResult> AuthorizeAsync(ClaimsPrincipal user, object? resource, string policyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(policyName, nameof(policyName));

        var policy = await _policyProvider.GetPolicyAsync(policyName).ConfigureAwait(false);
        if (policy == null)
        {
            throw new InvalidOperationException($"No policy found: {policyName}.");
        }
        return await this.AuthorizeAsync(user, resource, policy).ConfigureAwait(false);
    }
}
