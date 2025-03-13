namespace EntityFrameworkCore.PolicyEnforcement.Models;

/// <summary>
/// Configuration options for policy enforcement.
/// </summary>
public class PolicyEnforcementOptions
{
	/// <summary>
	/// Gets or sets whether policy enforcement should be applied to queries.
	/// Default value is true.
	/// </summary>
	public bool EnableForQueries { get; set; } = true;

	/// <summary>
	/// Gets or sets whether policy enforcement should be applied to commands (insert, update, delete).
	/// Default value is true.
	/// </summary>
	public bool EnableForCommands { get; set; } = true;

	/// <summary>
	/// Gets or sets whether policy violations should throw exceptions.
	/// If false, operations are silently prevented instead.
	/// Default value is true.
	/// </summary>
	public bool ThrowOnViolation { get; set; } = true;

	/// <summary>
	/// Gets or sets whether policy checks should be skipped for users in the admin role.
	/// Default value is false.
	/// </summary>
	public bool SkipPolicyCheckForAdmins { get; set; } = false;

	/// <summary>
	/// Gets or sets the name of the admin role for use with SkipPolicyCheckForAdmins.
	/// Default value is "Admin".
	/// </summary>
	public string AdminRoleName { get; set; } = "Admin";
}