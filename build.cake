#load "build/helpers.cake"
#addin nuget:?package=Cake.Docker&version=1.3.0

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var isLatest = Argument("tag-latest", true);
var isStable = EnvironmentVariable("GITHUB_REF")?.StartsWith("refs/tags/v") ?? false;

///////////////////////////////////////////////////////////////////////////////
// VERSIONING
///////////////////////////////////////////////////////////////////////////////

var packageVersion = string.Empty;
#load "build/version.cake"

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var solutionPath = File("./src/Sputter.sln");
var solution = ParseSolution(solutionPath);
var projects = GetProjects(solutionPath, configuration);
var artifacts = "./dist/";
var testResultsPath = MakeAbsolute(Directory(artifacts + "./test-results"));
var plugins = new List<string> { "Sputter.LibreHardwareMonitor", "Sputter.Synology" };

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
	// Executed BEFORE the first task.
	Information("Running tasks...");
	packageVersion = BuildVersion(fallbackVersion);
	if (FileExists("./build/.dotnet/dotnet.exe")) {
		Information("Using local install of `dotnet` SDK!");
		Context.Tools.RegisterFile("./build/.dotnet/dotnet.exe");
	}
	if (isLatest) {
		Information("Publishing images as 'latest' (if publishing is enabled)!");
	}
	if (isStable) {
		Information("Publishing images as 'stable' (if publishing is enabled)!");
	}
});

Teardown(ctx =>
{
	// Executed AFTER the last task.
	Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASK DEFINITIONS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
{
	// Clean solution directories.
	foreach(var path in projects.AllProjectPaths)
	{
		Information("Cleaning {0}", path);
		CleanDirectories(path + "/**/bin/" + configuration);
		CleanDirectories(path + "/**/obj/" + configuration);
	}
	foreach (var proj in projects.AllProjects) {
		Information(proj.Type);
	}
	Information("Cleaning common files...");
	CleanDirectory(artifacts);
});

Task("Restore")
	.Does(() =>
{
	// Restore all NuGet packages.
	Information("Restoring solution...");
	foreach (var project in projects.AllProjectPaths) {
		DotNetRestore(project.FullPath);
	}
});

Task("Build")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.Does(() =>
{
	Information("Building solution...");
	var settings = new DotNetBuildSettings {
		Configuration = configuration,
		NoIncremental = true,
		ArgumentCustomization = args => args.Append($"/p:Version={packageVersion}").Append("/p:AssemblyVersion=1.0.0.0")
	};
	DotNetBuild(solutionPath, settings);
});

Task("Run-Unit-Tests")
	.IsDependentOn("Build")
	.Does(() =>
{
	CreateDirectory(testResultsPath);
	if (projects.TestProjects.Any()) {

		var settings = new DotNetTestSettings {
			Configuration = configuration
		};

		foreach(var project in projects.TestProjects) {
			DotNetTest(project.Path.FullPath, settings);
		}
	}
});

Task("Publish-Runtime")
	.IsDependentOn("Build")
	.Does(() =>
{
	var projectDir = $"{artifacts}publish";
	CreateDirectory(projectDir);

	foreach (var project in projects.SourceProjects)
	{
		var projPath = project.Path.FullPath;
		Information("Publishing {0}", project.Name);
		DotNetPublish(projPath, new DotNetPublishSettings {
			OutputDirectory = $"{projectDir}/{project.Name}/dotnet-any",
			Configuration = configuration,
			PublishSingleFile = false,
			SelfContained = false,
			PublishTrimmed = false,
			NoBuild = true,
			NoRestore = true,
			ArgumentCustomization = args => args
				.Append("/p:UseAppHost=false")
				.Append("/p:StandalonePublish=true")
				.Append($"/p:Version={packageVersion}")
				.Append("/p:AssemblyVersion=1.0.0.0")
				.Append("/p:NoWarn=\"IL2104 IL2072 IL2087\"")
		});
		var runtimes = new[] { "win-x64", "linux-x64", "linux-arm64"};
		// plugins don't get native builds
		if (!plugins.Contains(project.Name)) {
			foreach (var runtime in runtimes) {
				var runtimeDir = $"{projectDir}/{project.Name}/{runtime}";
				// CreateDirectory(runtimeDir);
				Information("Publishing {0} for {1} runtime", project.Name, runtime);
				var settings = new DotNetPublishSettings {
					Runtime = runtime,
					Configuration = configuration,
					OutputDirectory = runtimeDir,
					PublishSingleFile = true,
					SelfContained = true,
					ArgumentCustomization = args => args
						.Append($"/p:Version={packageVersion}")
						.Append("/p:AssemblyVersion=1.0.0.0")
						// .Append("/p:AssemblyName=sputter")
				};
				DotNetPublish(projPath, settings);
				CreateDirectory($"{artifacts}archive");
				if (DirectoryExists(runtimeDir)) {
					Zip(runtimeDir, $"{artifacts}archive/sputter-{project.Name.Split('.').Last().ToLower()}-{runtime}.zip");
				}
			}
		}
	}
});

Task("Publish-Plugins")
	.IsDependentOn("Build")
	.Does(() =>
{
	var pluginsDir = $"{artifacts}plugins";
	CreateDirectory(pluginsDir);
	foreach(var project in projects.SourceProjects.Where(p => plugins.Contains(p.Name))) {
		DotNetPublish(project.Path.FullPath, new DotNetPublishSettings {
			OutputDirectory = pluginsDir + "/" + project.Name,
			Configuration = configuration,
			PublishSingleFile = false,
			SelfContained = false,
			PublishTrimmed = false,
			NoBuild = true,
			NoRestore = true,
		});
	}
	CreateDirectory($"{artifacts}archive");
	Zip(pluginsDir, $"{artifacts}archive/sputter-plugins.zip");
});

Task("NuGet")
    .IsDependentOn("Build")
    .Does(() =>
{
    Information("Building NuGet package");
    CreateDirectory(artifacts + "package/");
    var packSettings = new DotNetPackSettings {
        Configuration = configuration,
        NoBuild = true,
        OutputDirectory = $"{artifacts}package",
        ArgumentCustomization = args => args
            .Append($"/p:Version=\"{packageVersion}\"")
            .Append("/p:NoWarn=\"NU1701 NU1602\"")
    };
    /*foreach(var project in projects.SourceProjectPaths) {
        Information($"Packing {project.GetDirectoryName()}...");
        DotNetPack(project.FullPath, packSettings);
    }*/
    DotNetPack(solutionPath, packSettings);
});

Task("Build-Docker-Image")
	.WithCriteria(IsRunningOnUnix())
	.IsDependentOn("Publish-Runtime")
	.Does(() =>
{
    var dockerFileName = "publish.Dockerfile";
	Information("Building Docker image...");
	CopyFileToDirectory($"./build/{dockerFileName}", artifacts);
	var bSettings = new DockerImageBuildSettings {
        Tag = new[] { $"sputter/server:{packageVersion}"},
        File = artifacts + dockerFileName,
        BuildArg = new[] {$"package_version={packageVersion}"}
    };
	DockerBuild(bSettings, artifacts);
	DeleteFile(artifacts + dockerFileName);
});

Task("Publish-Docker-Image")
.IsDependentOn("Publish-Runtime")
.WithCriteria(IsRunningOnUnix())
.WithCriteria(() => !string.IsNullOrWhiteSpace(EnvironmentVariable("QUAY_TOKEN")))
//.WithCriteria(() => EnvironmentVariable("GITHUB_REF").StartsWith("refs/tags/v"))
.Does(() => {
    var token = EnvironmentVariable("QUAY_TOKEN");
    DockerLogin(new DockerRegistryLoginSettings{
        Password = token,
        Username = EnvironmentVariable("QUAY_USER") ?? "quay"
    }, "quay.io");
    var dockerFileName = "publish.Dockerfile";
	Information("Building Docker image...");
	CopyFileToDirectory($"./build/{dockerFileName}", artifacts);
	var tags = new List<string> { $"quay.io/sputter/server:{packageVersion}" };
	if (isLatest) {
		// Don't want to add named tags yet
		tags.Add("quay.io/sputter/server:latest");
	}
	if (isStable) {
		tags.Add("quay.io/sputter/server:stable");
	}
	var bSettings = new DockerBuildXBuildSettings {
        Tag = tags.ToArray(),
        File = artifacts + dockerFileName,
        BuildArg = new[] {$"package_version={packageVersion}"},
		Platform = new[] { "linux/arm64", "linux/arm", "linux/amd64"},
		Push = true,
		ArgumentCustomization = args => args
            .Append($"--provenance=false")
    };
	DockerBuildXBuild(bSettings, artifacts);
	DeleteFile(artifacts + dockerFileName);
});

#load "build/publish.cake"

Task("Default")
	.IsDependentOn("Build");

Task("Publish")
	.IsDependentOn("Publish-Runtime")
	.IsDependentOn("Publish-Plugins")
	.IsDependentOn("NuGet")
	.IsDependentOn("Build-Docker-Image");

Task("Release")
	.IsDependentOn("Publish")
	.IsDependentOn("Publish-NuGet-Package")
	.IsDependentOn(string.IsNullOrWhiteSpace(EnvironmentVariable("QUAY_TOKEN")) ? "Build-Docker-Image" : "Publish-Docker-Image");

RunTarget(target);