var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var DotNetProjects = new List<string>()
{
	"Okta.AspNet"
};

var DotNetStandardProjects = new List<string>()
{
	"Okta.AspNet.Abstractions"
};

Task("Clean")
.Does(() =>
{
    CleanDirectory("./artifacts/");
    CleanDirectory("./packages/");

    GetDirectories("./**/bin")
		.Concat(GetDirectories("./**/obj"))
        .ToList()
        .ForEach(d => CleanDirectory(d));
});

Task("Default")
    .IsDependentOn("Clean");
    //.IsDependentOn("Restore")
    //.IsDependentOn("Build")
    //.IsDependentOn("Test")
    //.IsDependentOn("Pack");

// Run the specified (or default) target
RunTarget(target);
