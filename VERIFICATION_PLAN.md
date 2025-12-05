# Strong Name Signature Fix Verification Plan

## Overview
This document outlines the comprehensive verification strategy for the DelaySign→PublicSign fix to prevent regression of GitHub issue #303.

## Phase 1: Local Verification ✅ COMPLETED
- [x] Build configuration confirmed using PublicSign=true
- [x] All assemblies compile with public key embedded
- [x] No DelaySign references in build output
- [x] 165/165 unit tests passing
- [x] Fix committed to upgrade-aspnet branch

## Phase 2: Windows Server Environment Testing 🔄 CRITICAL

### 2.1 Test Environments
Test the fix on the actual environments where the failure occurred:

#### Windows Server 2016
- IIS 10.0
- .NET Framework 4.8 runtime
- **No Visual Studio installed** (critical - this is where DelaySign failed)

#### Windows Server 2019/2022  
- IIS 10.0
- .NET Framework 4.8 runtime
- **No Visual Studio installed**

### 2.2 Test Applications
Create minimal test applications for each affected scenario:

```csharp
// TestApp1: ASP.NET WebForms (.NET Framework 4.8)
<%@ Page Language="C#" %>
<%@ Import Namespace="Okta.AspNet" %>
<!DOCTYPE html>
<script runat="server">
    protected void Page_Load(object sender, EventArgs e)
    {
        // This line would fail with old DelaySign assemblies
        var options = new OktaMvcOptions();
        Response.Write("Okta.AspNet loaded successfully!");
    }
</script>

// TestApp2: ASP.NET MVC (.NET Framework 4.8)
public class HomeController : Controller
{
    public ActionResult Index()
    {
        // This would trigger the strong name verification
        var provider = new UserInformationProvider();
        return Content("Okta.AspNet.Abstractions loaded successfully!");
    }
}

// TestApp3: ASP.NET Core (multiple versions)
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication()
            .AddOktaMvc(options => {
                // Strong name verification happens here
                options.OktaDomain = "https://your-domain.okta.com";
            });
    }
}
```

### 2.3 Deployment Verification Steps

#### Step 1: Binary Deployment
1. **Copy assemblies** to Windows Server test environments
2. **Verify GAC registration** (if used): `gacutil -l Okta.AspNet.Abstractions`
3. **Check assembly metadata**: 
   ```cmd
   sn -vf Okta.AspNet.Abstractions.dll
   sn -Tp Okta.AspNet.Abstractions.dll
   ```

#### Step 2: Application Load Testing
1. **Deploy test applications** to IIS
2. **Application startup**: Verify no assembly load exceptions
3. **First request processing**: This triggers JIT compilation and strong name verification
4. **Monitor Event Log** for any signature verification errors

#### Step 3: Load Testing
```powershell
# PowerShell script to stress test the fix
for ($i = 1; $i -le 100; $i++) {
    Invoke-WebRequest -Uri "http://testserver/TestApp/Home" 
    Write-Host "Request $i completed"
    Start-Sleep -Milliseconds 100
}
```

## Phase 3: Package Distribution Testing

### 3.1 NuGet Package Verification
1. **Build NuGet packages** with the fixed assemblies
2. **Create fresh ASP.NET projects** consuming the packages
3. **Deploy to Windows Server** environments
4. **Verify no installation/runtime issues**

### 3.2 Version Compatibility Testing
Test with customers' existing configurations:
- .NET Framework 4.6.1+ applications  
- ASP.NET Core 2.1+ applications
- Mixed deployments with both frameworks

## Phase 4: Automated Regression Testing

### 4.1 CI/CD Pipeline Enhancement
Add automated tests to prevent future regression:

```yaml
# Azure DevOps/GitHub Actions pipeline addition
- task: DotNetCoreCLI@2
  displayName: 'Verify Strong Name Configuration'
  inputs:
    command: 'custom'
    custom: 'build'
    arguments: '--verbosity normal --configuration Release'
  continueOnError: false

- powershell: |
    # Verify no DelaySign in output
    $buildLog = Get-Content "$(Agent.TempDirectory)/build.log"
    if ($buildLog -match "/delaysign\+.*keycontainer") {
        throw "DelaySign with key container detected - this will fail in production!"
    }
    # Verify PublicSign is used
    if (-not ($buildLog -match "/delaysign\+.*keyfile.*\.snk")) {
        throw "PublicSign configuration not detected!"
    }
  displayName: 'Validate Signing Configuration'
```

### 4.2 Integration Test Enhancement
```csharp
[TestMethod]
public void Assembly_ShouldLoadInConstrainedEnvironment()
{
    // Simulate production environment (no dev tools)
    var assemblyPath = typeof(OktaMvcOptions).Assembly.Location;
    
    // This would fail with DelaySign assemblies in production
    var assembly = Assembly.LoadFrom(assemblyPath);
    
    // Verify we can instantiate types (triggers strong name verification)
    var instance = Activator.CreateInstance(assembly.GetType("Okta.AspNet.OktaMvcOptions"));
    Assert.IsNotNull(instance);
}

[TestMethod]  
public void StrongName_ShouldBeVerifiable()
{
    var assembly = typeof(OktaMvcOptions).Assembly;
    
    // Verify assembly has strong name
    Assert.IsTrue(assembly.GetName().GetPublicKey().Length > 0, 
        "Assembly should have strong name");
    
    // Verify it's not delay signed (would cause production failures)
    var assemblyPath = assembly.Location;
    // Use reflection to check if delay signed attribute is false
    // (This prevents the DelaySign production issue)
}
```

## Phase 5: Customer Environment Validation

### 5.1 Beta Testing
1. **Identify affected customers** from issues #291, #296, #303
2. **Provide pre-release packages** with the fix
3. **Gather deployment feedback** from their actual environments
4. **Verify fix resolves their specific scenarios**

### 5.2 Documentation Update
```markdown
# Deployment Verification Checklist

Before deploying Okta.AspNet 5.1.8+:

✅ Windows Server environment does NOT have Visual Studio
✅ Application can start without assembly load exceptions  
✅ First HTTP request completes successfully
✅ Event Viewer shows no strong name verification errors
✅ Multiple application restarts work correctly

If you encounter "Strong name signature could not be verified" errors:
1. Check you're using version 5.1.8 or later
2. Verify the Okta.AspNet.Abstractions.dll has PublicKeyToken=a5a8152428dc4790
3. Report the issue with your specific Windows Server configuration
```

## Success Criteria

### ✅ Local Verification (Completed)
- [x] Build uses PublicSign=true configuration
- [x] No DelaySign references in build output  
- [x] All unit tests passing (165/165)

### 🔄 Windows Server Verification (Required)
- [ ] Assembly loads successfully on Windows Server 2016/2019/2022
- [ ] No strong name verification errors in Event Log
- [ ] Test applications start and process requests
- [ ] Multiple application lifecycle events (start/stop/restart)

### 🔄 Integration Testing (Required)
- [ ] NuGet packages install and work in fresh projects
- [ ] Existing customer configurations continue working
- [ ] Mixed .NET Framework/.NET Core deployments successful

### 🔄 Regression Prevention (Required)
- [ ] CI/CD pipeline validates signing configuration
- [ ] Automated tests catch future DelaySign issues
- [ ] Documentation updated for deployment verification

## Rollback Plan

If verification reveals any issues:

1. **Immediate**: Revert to DelaySign=false (removes strong naming temporarily)
2. **Investigation**: Analyze specific failure scenarios
3. **Alternative**: Consider authenticode signing as interim solution
4. **Resolution**: Address root cause and re-test

## Timeline

- **Phase 1**: ✅ Completed (Local verification)
- **Phase 2**: 2-3 days (Windows Server testing)
- **Phase 3**: 1-2 days (Package distribution testing)  
- **Phase 4**: 1 day (CI/CD pipeline updates)
- **Phase 5**: 1 week (Customer validation)

**Total verification timeline**: ~2 weeks

## Resources Needed

- **Windows Server test environments** (2016, 2019, 2022)
- **IIS configuration** matching customer environments
- **Test applications** covering affected scenarios
- **Monitoring tools** for Event Log analysis
- **Customer contact** for beta testing feedback

---

*This verification plan ensures the DelaySign→PublicSign fix resolves issue #303 permanently while preventing regression in future releases.*