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
});

Task("Build")
.IsDependentOn("Restore")
.Does(() =>
{
    Projects.ForEach(name =>
    {
        Console.WriteLine($"\nBuilding {name}");
       
        if(circleCiEnabled && name == "Okta.AspNet.Abstractions")
        {
            DotNetCoreBuild($"./{name}", new DotNetCoreBuildSettings
            {
                Configuration = configuration,
                Framework = "netstandard2.0", 
            });
        }
        else
        {
            DotNetCoreBuild($"./{name}", new DotNetCoreBuildSettings
            {
                Configuration = configuration,
            });
        }
        
    });
});

Task("RunTests")
.IsDependentOn("Restore")
.IsDependentOn("Build")
.Does(() =>
{
    Projects
    .Where(name => name.EndsWith(".Test")) // For now, we won't run integration tests in CI
    .ToList()
    .ForEach(name => {
        DotNetCoreTest(string.Format("./{0}/{0}.csproj", name));
    });
});

// Note: The Strongname task has been removed.
// The project now uses PublicSign instead of DelaySign, which embeds the public key
// at compile time without requiring a private key or post-build sn.exe step.
// This is Microsoft's recommended approach for open-source projects.
// See: https://learn.microsoft.com/en-us/dotnet/standard/library-guidance/strong-naming

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
        
        if(circleCiEnabled && name == "Okta.AspNet.Abstractions") 
        {
            var msBuildSettings = new DotNetCoreMSBuildSettings();
            msBuildSettings.SetTargetFramework("netstandard2.0");

            DotNetCorePack($"./{name}", new DotNetCorePackSettings
            {
                Configuration = configuration,
                OutputDirectory = "./artifacts",
                MSBuildSettings = msBuildSettings,
				NoBuild = true,
            });
        } 
        else
        {
            DotNetCorePack($"./{name}", new DotNetCorePackSettings
            {
                Configuration = configuration,
                OutputDirectory = "./artifacts",
                NoBuild = true,
            });
        }
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
    
// Run the specified (or default) target
RunTarget(target);
