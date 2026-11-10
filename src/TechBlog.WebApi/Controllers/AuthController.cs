using Asp.Versioning;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using TechBlog.Application.Auth;
using TechBlog.Application.Auth.Commands.Login;
using TechBlog.Application.Auth.Commands.RefreshToken;

namespace TechBlog.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiVersionNeutral]
[AllowAnonymous]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<LoginResponse>> Login(LoginCommand command, CancellationToken ct) =>
        Ok(await mediator.Send(command, ct));

    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    public async Task<ActionResult<LoginResponse>> Refresh(RefreshTokenCommand command, CancellationToken ct) =>
        Ok(await mediator.Send(command, ct));
}
