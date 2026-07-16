namespace TechBlog.Domain.Common;

/// <summary>Thrown when an operation would violate a domain invariant -
/// caught at the WebApi boundary and turned into a 400, not a 500.</summary>
public sealed class DomainException(string message) : Exception(message);
