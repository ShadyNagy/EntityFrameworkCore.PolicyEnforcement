namespace EntityFrameworkCore.PolicyEnforcement.Models;

public class PolicyEnforcementOptions
{
	public bool EnableForQueries { get; set; } = true;
	public bool EnableForCommands { get; set; } = true;
	public bool ThrowOnViolation { get; set; } = true;
	public bool SkipPolicyCheckForAdmins { get; set; } = false;
	public string AdminRoleName { get; set; } = "Admin";
}