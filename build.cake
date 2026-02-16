#addin nuget:?package=Cake.Figlet&version=1.3.1

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

Boolean.TryParse(EnvironmentVariable("CIRCLE_CI"), out var circleCiEnabled);
Console.WriteLine($"\n Circle Ci enabled: {circleCiEnabled}");
Console.WriteLine($"\n Jenkins build: {BuildSystem.IsRunningOnJenkins}");

var Projects = new List<string>()
{
    "Okta.AspNet.Abstractions",
    "Okta.AspNet.Abstractions.Test",
    "Okta.AspNet",
    "Okta.AspNet.Test",
    "Okta.AspNetCore",
    "Okta.AspNetCore.Test"
};

var IntegrationTestProjects = new List<string>()
{
    "Okta.AspNet.WebApi.IntegrationTest",
    "Okta.AspNet.Mvc.IntegrationTest",
    "Okta.AspNetCore.WebApi.IntegrationTest",
    "Okta.AspNetCore.Mvc.IntegrationTest"
};

// Ignoring .NET 4.5.2 projects as it is causing issues with travis.
// https://github.com/okta/okta-aspnet/issues/40
var netCoreProjects = new List<string>()
{
    "Okta.AspNet.Abstractions",
    "Okta.AspNet.Abstractions.Test",
    "Okta.AspNet",
    "Okta.AspNet.Test",
    "Okta.AspNetCore",
    "Okta.AspNetCore.Test"
};

if(circleCiEnabled) 
{
    Projects = netCoreProjects;
}

Task("Clean").Does(() =>
{
    Console.WriteLine("Removing ./artifacts");
    CleanDirectory("./artifacts/");
    Console.WriteLine("Removing nested bin and obj directories");
    GetDirectories("./**/bin")
	.Concat(GetDirectories("./**/obj"))
        .ToList()
        .ForEach(d => CleanDirectory(d));
});

Task("Restore")
.IsDependentOn("Clean")
.Does(() =>
{
    Projects.ForEach(name =>
    {
        Console.WriteLine($"\nRestoring packages for {name}");
        DotNetCoreRestore($"./{name}");
    });
    
    // Also restore integration test projects
    IntegrationTestProjects.ForEach(name =>
    {
        Console.WriteLine($"\nRestoring packages for {name}");
        DotNetCoreRestore($"./{name}");
    });
});

Task("Build")
.IsDependentOn("Restore")
.Does(() =>
{
    Projects.ForEach(name =>
    {
        Console.WriteLine($"\nBuilding {name}");
       
        DotNetCoreBuild($"./{name}", new DotNetCoreBuildSettings
        {
            Configuration = configuration,
        });
        
    });
    
    // Also build integration test projects
    IntegrationTestProjects.ForEach(name =>
    {
        Console.WriteLine($"\nBuilding {name}");
        DotNetCoreBuild($"./{name}", new DotNetCoreBuildSettings
        {
            Configuration = configuration,
        });
    });
});

Task("RunTests")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.Does(() =>
{
    Projects
    .Where(name => name.EndsWith(".Test"))
    .ToList()
    .ForEach(name => {
        DotNetCoreTest(string.Format("./{0}/{0}.csproj", name));
    });
});

Task("RunIntegrationTests")
.IsDependentOn("Build")
.Does(() =>
{
    // Check if Okta credentials are available
    var hasOktaCredentials = !string.IsNullOrEmpty(EnvironmentVariable("OKTA_CLIENT_OKTADOMAIN")) &&
                             !string.IsNullOrEmpty(EnvironmentVariable("OKTA_CLIENT_CLIENTID"));
    
    if (!hasOktaCredentials)
    {
        Console.WriteLine("⚠️  Skipping integration tests - OKTA_CLIENT_* environment variables not set");
        return;
    }

    Console.WriteLine("✓ Running integration tests with Okta credentials");
    Console.WriteLine($"   Domain: {EnvironmentVariable("OKTA_CLIENT_OKTADOMAIN")}");
    Console.WriteLine($"   ClientId: {EnvironmentVariable("OKTA_CLIENT_CLIENTID")?.Substring(0, 8)}...");
    
    IntegrationTestProjects.ForEach(name => {
        Console.WriteLine($"\n========================================");
        Console.WriteLine($"Running integration tests for {name}");
        Console.WriteLine($"========================================");
        DotNetCoreTest(string.Format("./{0}/{0}.csproj", name), new DotNetCoreTestSettings
        {
            Configuration = configuration,
            NoBuild = false, // Build tests to ensure they're up to date
            Verbosity = DotNetCoreVerbosity.Normal,
        });
    });
});

Task("PackNuget")
.IsDependentOn("RunTests")
.Does(() =>
{
    Projects
    .Where(name => !name.Contains(".Test"))
    .ToList()
    .ForEach(name =>
    {
        Console.WriteLine($"\nCreating NuGet package for {name}");
        
        DotNetCorePack($"./{name}", new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = "./artifacts",
            NoBuild = true,
        });
    });
});

Task("Info")
.Does(() => 
{
    Information(Figlet("Okta.AspNet"));

    var cakeVersion = typeof(ICakeContext).Assembly.GetName().Version.ToString();

    Information("Building using {0} version of Cake", cakeVersion);
});

Task("Default")
    .IsDependentOn("Info")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("RunTests")
    .IsDependentOn("PackNuget");

Task("DefaultIT")
    .IsDependentOn("Info")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("RunTests")
    .IsDependentOn("RunIntegrationTests")
    .IsDependentOn("PackNuget");
    
// Run the specified (or default) target
RunTarget(target);
