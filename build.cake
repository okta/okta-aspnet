var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var Projects = new List<string>()
{
	"Okta.AspNet.Abstractions",
    "Okta.AspNet"
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

Task("PackNuget")
.IsDependentOn("Build")
.Does(() =>
{
    Projects.ForEach(name =>
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
    // TODO: Test
    .IsDependentOn("PackNuget");

// Run the specified (or default) target
RunTarget(target);
