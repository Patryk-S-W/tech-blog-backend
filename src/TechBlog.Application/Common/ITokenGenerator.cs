using TechBlog.Domain.Users;

namespace TechBlog.Application.Common;

public interface ITokenGenerator
{
    string CreateToken(User user);
}
