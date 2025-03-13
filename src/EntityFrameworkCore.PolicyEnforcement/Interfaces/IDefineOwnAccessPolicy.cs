namespace EntityFrameworkCore.PolicyEnforcement.Interfaces;

/// <summary>
/// Interface for entities that define their own access policy logic.
/// </summary>
public interface IDefineOwnAccessPolicy
{
	/// <summary>
	/// Determines if the current user can access this entity for the specified operation.
	/// </summary>
	/// <param name="userContext">The user context containing user information.</param>
	/// <param name="operation">The operation being performed (e.g., "Create", "Read", "Update", "Delete").</param>
	/// <returns>True if access is allowed; otherwise, false.</returns>
	bool CanAccess(IUserContext userContext, string operation);
}