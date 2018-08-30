#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var nugetDir = Argument("nugetdir", @"..\..\NuGet");
var configuration = Argument("configuration", "Release");
var symbols = false;
var suffix = "";

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory("./bin") + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(buildDir);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore("Autofac.Extras.UnitTestExtensions.sln");
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild("Autofac.Extras.UnitTestExtensions.sln", settings => settings.SetConfiguration(configuration));    
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    NUnit3("./src/**/bin/" + configuration + "/*.Tests.dll", new NUnit3Settings { NoResults = true });
});

Task("NuGet-Pack")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() => 
    {
		var args = "pack " + (symbols ? " --include-symbols " : " ") + (string.IsNullOrEmpty(suffix) ?  " " : "--version-suffix \"" + suffix + "\"") + " --configuration " + configuration + " {0}";
		StartProcess("dotnet", string.Format(args, @"Autofac.Extras.UnitTestExtensions\Autofac.Extras.UnitTestExtensions.csproj"));		
    });

Task("NuGet-Publish")
    .IsDependentOn("NuGet-Pack")
    .Does(() =>
    {        
        MoveFiles(@"./**/bin/" + configuration + "/*.nupkg", nugetDir);
    });

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("NuGet-Publish");

Task("Release")
	.Does(() =>
	{
		configuration = Argument("configuration", "Release");
		symbols = false;
		suffix = "";
		RunTarget("Default");
	});

Task("Debug")
	.Does(() =>
	{
		configuration = Argument("configuration", "Debug");	
		symbols = true;
		suffix = "ci-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
		RunTarget("Default");
	});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
