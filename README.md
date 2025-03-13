[![publish to nuget](https://github.com/ShadyNagy/EntityFrameworkCore.PolicyEnforcement/actions/workflows/nugt-publish.yml/badge.svg)](https://github.com/ShadyNagy/EntityFrameworkCore.PolicyEnforcement/actions/workflows/nugt-publish.yml)
[![EntityFrameworkCore.PolicyEnforcement on NuGet](https://img.shields.io/nuget/v/EntityFrameworkCore.PolicyEnforcement?label=EntityFrameworkCore.PolicyEnforcement)](https://www.nuget.org/packages/EntityFrameworkCore.PolicyEnforcement/)
[![NuGet](https://img.shields.io/nuget/dt/EntityFrameworkCore.PolicyEnforcement)](https://www.nuget.org/packages/EntityFrameworkCore.PolicyEnforcement)
[![License](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/ShadyNagy/EntityFrameworkCore.PolicyEnforcement/blob/main/LICENSE)
[![Sponsor](https://img.shields.io/badge/Sponsor-ShadyNagy-brightgreen?logo=github-sponsors)](https://github.com/sponsors/ShadyNagy)

# EntityFrameworkCore.PolicyEnforcement

## 📌 Introduction

A .NET library for enforcing access control policies in Entity Framework Core applications with dependency injection support.

### 📌 Key Features:  

- 🔒 **Declarative Access Control**: Define policies at the entity level using a fluent API
- 🧩 **Multiple Policy Types**: Support for role-based, permission-based, ownership-based, and property-based policies
- ⚡ **Automatic Enforcement**: Policies applied automatically to both queries and commands
- 🔌 **Extensible**: Build custom policy types for specific business requirements
- 🏗️ **Clean Architecture Friendly**: Designed to work well with domain-driven and clean architecture approaches
- 🧪 **Testable**: Easy to mock and test in isolation
- 🔄 **Chainable Policies**: Combine multiple policies with AND/OR operators
- 🧠 **Smart Caching**: Performance optimized with compiled expression caching

## 📥 Installation

```bash
dotnet add package EntityFrameworkCore.PolicyEnforcement
```

## 🚀 Quick Start: 

### 1. Configure in your DbContext

```csharp
public class ApplicationDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(connectionString)
                     .UsePolicyEnforcement();
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>()
            .HasAccessPolicy(policy => 
                policy.RequireOwnership(d => d.OwnerId)
                    .Or(policy.RequireRole("Admin"))
            );
    }
}
```

### 2. Set up the User Context

Create a class implementing `IUserContext`:

```csharp
public class CurrentUserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public string? GetCurrentUserId() => 
        _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    
    public bool IsInRole(string role) => 
        _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    
    public bool HasPermission(string permission) => 
        _httpContextAccessor.HttpContext?.User?.HasClaim(c => 
            c.Type == "Permission" && c.Value == permission) ?? false;
}
```

### 3. Configure in Startup

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddScoped<IUserContext, CurrentUserContext>();
    
    services.AddDbContext<ApplicationDbContext>(options => 
        options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
               .UsePolicyEnforcement(opt => 
               {
                   opt.EnableForQueries = true;
                   opt.EnableForCommands = true;
                   opt.ThrowOnViolation = true;
               }));
}
```

### 4. Set User Context Before Operations

```csharp
public class DocumentService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IUserContext _userContext;
    
    public DocumentService(ApplicationDbContext dbContext, IUserContext userContext)
    {
        _dbContext = dbContext;
        _userContext = userContext;
    }
    
    public async Task<List<Document>> GetUserDocumentsAsync()
    {
        // Set the user context before querying
        _dbContext.SetUserContext(_userContext);
        
        // Policy is automatically applied
        return await _dbContext.Documents.ToListAsync();
    }
}
```

## Policy Types

### Role-Based Policies

```csharp
modelBuilder.Entity<Payroll>()
    .HasAccessPolicy(policy => policy.RequireRole("HR"));
```

### Permission-Based Policies

```csharp
modelBuilder.Entity<Customer>()
    .HasAccessPolicy(policy => policy.RequirePermission("customers.read"));
```

### Ownership-Based Policies

```csharp
modelBuilder.Entity<UserProfile>()
    .HasAccessPolicy(policy => policy.RequireOwnership(p => p.UserId));
```

### Property-Based Policies

```csharp
modelBuilder.Entity<Document>()
    .HasAccessPolicy(policy => policy.RequireProperty(d => d.IsPublic));
```

### Combined Policies

```csharp
modelBuilder.Entity<Project>()
    .HasAccessPolicy(policy => 
        policy.RequireOwnership(p => p.OwnerId)
            .Or(policy.RequireRole("Manager"))
            .Or(policy.RequireProperty(p => p.IsPublic))
    );
```

### Operation-Specific Policies

```csharp
modelBuilder.Entity<Ticket>()
    .HasAccessPolicy(policy => policy.RequireRole("Support"), "Default")
    .HasAccessPolicy(policy => policy.RequireRole("SupportLead"), "Update")
    .HasAccessPolicy(policy => policy.RequireRole("Manager"), "Delete");
```

## Advanced Usage

### Custom Policies

```csharp
modelBuilder.Entity<FinancialRecord>()
    .HasAccessPolicy(policy => policy.Custom(userContext => 
        fr => fr.Amount < 1000 || userContext.IsInRole("FinancialApprover")
    ));
```

### Self-Checking Entities

```csharp
public class Team : IDefineOwnAccessPolicy
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<TeamMember> Members { get; set; }
    
    public bool CanAccess(IUserContext userContext, string operation)
    {
        var userId = userContext.GetCurrentUserId();
        if (string.IsNullOrEmpty(userId)) return false;
        
        // Allow members to read, but only admins to modify
        if (operation == "Read")
            return Members.Any(m => m.UserId == userId);
            
        return Members.Any(m => m.UserId == userId && m.IsAdmin);
    }
}
```

## 🔗 License
This project is licensed under the **MIT License**. See the [LICENSE](LICENSE) file for details.

---

## 🙌 Contributing
🎯 Found a bug or have an idea for improvement?  
Feel free to **open an issue** or **submit a pull request**!  
🔗 [GitHub Issues](https://github.com/ShadyNagy/EntityFrameworkCore.PolicyEnforcement/issues)

---

## ⭐ Support the Project
If you find this package useful, **give it a star ⭐ on GitHub** and **share it with others!** 🚀
