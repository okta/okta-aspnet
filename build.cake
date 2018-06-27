var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var Projects = new List<string>()
{
	"Okta.AspNet.Abstractions",
    "Okta.AspNet.Abstractions.Test",
    "Okta.AspNet",
	"Okta.AspNet.Test",
	"Okta.AspNetCore",
	"Okta.AspNetCore.Test"
};

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

        DotNetCoreBuild($"./{name}", new DotNetCoreBuildSettings
        {
            Configuration = configuration
        });
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
            OutputDirectory = "./artifacts"
        });
    });
});

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build")
    .IsDependentOn("RunTests")
    .IsDependentOn("PackNuget");

// Run the specified (or default) target
RunTarget(target);
