using FluentValidation;
using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Common;
using TechBlog.Domain.Users;

namespace TechBlog.Application.Auth.Commands.Login;

public sealed record LoginCommand(string Username, string Password) : IRequest<LoginResponse>;

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
    ITokenGenerator tokenGenerator,
    IRefreshTokenRepository refreshTokenRepository,
    IUnitOfWork unitOfWork)
    : IRequestHandler<LoginCommand, LoginResponse>
{
    public async ValueTask<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var username = Username.Create(request.Username);
        var user = await userRepository.GetByUsernameAsync(username, cancellationToken);

        if (user is null)
            throw new UnauthorizedAccessException("Invalid username or password.");

        if (user.IsLockedOut)
            throw new UnauthorizedAccessException("Account is locked. Try again later.");

        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await userRepository.UpdateAsync(user, cancellationToken);
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        user.ResetFailedLoginAttempts();
        await userRepository.UpdateAsync(user, cancellationToken);

        var accessToken = tokenGenerator.CreateAccessToken(user);
        var refreshToken = tokenGenerator.CreateRefreshToken();

        var refreshTokenEntity = Domain.Users.RefreshToken.Create(refreshToken, user.Id, TimeSpan.FromDays(7));
        await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(accessToken, refreshToken);
    }
}
