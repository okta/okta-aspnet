var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var DotnetProjects = new List<string>()
{
	"Okta.AspNet"
};

var DotnetStandardProjects = new List<string>()
{
	"Okta.AspNet.Abstractions"
};

Task("Clean").Does(() =>
{
    CleanDirectory("./artifacts/");
    CleanDirectory("./packages/");

    GetDirectories("./**/bin")
		.Concat(GetDirectories("./**/obj"))
        .ToList()
        .ForEach(d => CleanDirectory(d));
});

Task("DotnetStandardRestore").Does(() => {
    DotnetStandardProjects.ForEach(projectNameWithoutExtension => DotNetCoreRestore($"./{projectNameWithoutExtension}"));
});

Task("DotnetStandardBuild")
.IsDependentOn("DotnetStandardRestore")
.Does(() => {
    DotnetStandardProjects.ForEach(projectNameWithoutExtension => {
        Console.WriteLine("Building project ", projectNameWithoutExtension);

        DotNetCoreBuild($"./{projectNameWithoutExtension}", new DotNetCoreBuildSettings
        {
            Configuration = configuration
        });
    });
});

Task("DotnetStandardCreateNugetPackages")
.IsDependentOn("DotnetStandardBuild")
.Does(() => {
    DotnetStandardProjects.ForEach(projectNameWithoutExtension => {
        Console.WriteLine("Creating NuGet package for project ", projectNameWithoutExtension);

        DotNetCorePack($"./{projectNameWithoutExtension}", new DotNetCorePackSettings
        {
            Configuration = configuration,
            OutputDirectory = "./artifacts/"
        });
    });
});

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("DotnetStandardBuild")
    .IsDependentOn("DotnetStandardCreateNugetPackages");

// Run the specified (or default) target
RunTarget(target);
