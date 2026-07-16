using FluentValidation;
using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Users;

namespace TechBlog.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Username, string Password) : IRequest<string>;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenGenerator tokenGenerator)
    : IRequestHandler<LoginCommand, string>
{
    public async ValueTask<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var username = Username.Create(request.Username);
        var user = await userRepository.GetByUsernameAsync(username, cancellationToken);

        // Deliberately the same failure for "no such user" and "wrong
        // password" - the original code returned two different messages
        // ("User not found." / "Wrong password."), which lets an attacker
        // enumerate valid usernames by watching which error comes back.
        // Flagging this as a real behavior change, not a silent one.
        if (user is null || !passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid username or password.");

        return tokenGenerator.CreateToken(user);
    }
}
