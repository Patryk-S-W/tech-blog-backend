using FluentValidation;
using Mediator;
using TechBlog.Domain.Common;
using TechBlog.Domain.Users;

namespace TechBlog.Application.Auth.Commands.Register;

public sealed record RegisterCommand(string Username, string Password) : IRequest<int>;

public sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MaximumLength(Username.MaxLength);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}

public sealed class RegisterCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
    : IRequestHandler<RegisterCommand, int>
{
    public async ValueTask<int> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var username = Username.Create(request.Username);

        if (await userRepository.ExistsAsync(username, cancellationToken))
            throw new DomainException("User already exists.");

        var passwordHash = passwordHasher.Hash(request.Password);
        var user = User.Register(username, passwordHash);

        await userRepository.AddAsync(user, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
