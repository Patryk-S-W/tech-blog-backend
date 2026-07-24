using TechBlog.Domain.Users;

namespace TechBlog.Application.Common;

public interface ITokenGenerator
{
    string CreateAccessToken(User user);
    string CreateRefreshToken();
}
