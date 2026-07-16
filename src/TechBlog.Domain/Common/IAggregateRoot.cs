namespace TechBlog.Domain.Common;

/// <summary>Marker for aggregate roots - the only entities that get their own
/// repository and the only ones referenced directly from outside their
/// aggregate (everything else inside is reached only through the root).</summary>
public interface IAggregateRoot;
