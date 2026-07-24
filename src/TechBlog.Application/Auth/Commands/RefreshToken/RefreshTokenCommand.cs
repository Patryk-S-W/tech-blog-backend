using FluentValidation;
using Mediator;
using TechBlog.Application.Common;
using TechBlog.Domain.Users;

namespace TechBlog.Application.Auth.Commands.RefreshToken;

public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<LoginResponse>;

public sealed class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public sealed class RefreshTokenCommandHandler(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenGenerator tokenGenerator,
    Domain.Common.IUnitOfWork unitOfWork)
    : IRequestHandler<RefreshTokenCommand, LoginResponse>
{
    public async ValueTask<LoginResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, cancellationToken)
            ?? throw new UnauthorizedAccessException("Invalid refresh token.");

        if (storedToken.IsExpired)
            throw new UnauthorizedAccessException("Refresh token has expired.");

        var user = await userRepository.GetByIdAsync(storedToken.UserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("User not found.");

        await refreshTokenRepository.RevokeAllForUserAsync(user.Id, cancellationToken);

        var newAccessToken = tokenGenerator.CreateAccessToken(user);
        var newRefreshToken = tokenGenerator.CreateRefreshToken();

        var refreshTokenEntity = Domain.Users.RefreshToken.Create(newRefreshToken, user.Id, TimeSpan.FromDays(7));
        await refreshTokenRepository.AddAsync(refreshTokenEntity, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new LoginResponse(newAccessToken, newRefreshToken);
    }
}
