using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechBlog.Application.Auth.Commands.Login;
using TechBlog.Application.Auth.Commands.Register;

namespace TechBlog.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
public sealed class AuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<int>> Register(RegisterCommand command, CancellationToken ct) =>
        Ok(await mediator.Send(command, ct));

    [HttpPost("login")]
    public async Task<ActionResult<string>> Login(LoginCommand command, CancellationToken ct) =>
        Ok(await mediator.Send(command, ct));
}
