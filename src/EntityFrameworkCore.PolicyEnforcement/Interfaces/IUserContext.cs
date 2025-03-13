namespace EntityFrameworkCore.PolicyEnforcement.Interfaces;

/// <summary>
/// Represents the context of the current user for policy evaluation.
/// </summary>
public interface IUserContext
{
	/// <summary>
	/// Gets the ID of the current user.
	/// </summary>
	/// <returns>The current user's ID, or null if not authenticated.</returns>
	string? GetCurrentUserId();

	/// <summary>
	/// Determines whether the current user is in the specified role.
	/// </summary>
	/// <param name="role">The role to check.</param>
	/// <returns>True if the user is in the role; otherwise, false.</returns>
	bool IsInRole(string role);

	/// <summary>
	/// Determines whether the current user has the specified permission.
	/// </summary>
	/// <param name="permission">The permission to check.</param>
	/// <returns>True if the user has the permission; otherwise, false.</returns>
	bool HasPermission(string permission);
}