using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MinVer;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.Tools.DotNet.DotNetTasks;


[SuppressMessage("ReSharper", "AllUnderscoreLocalParameterName")]
partial class Build : NukeBuild
{
    const int DegreeOfParallelism = 3; // we have 4 cores on the build server


    [Parameter] readonly Configuration Configuration;

    [Secret] [Parameter("GitHub API token")] [CanBeNull] readonly string GitHubToken;

    [CI] [CanBeNull] readonly GitHubActions GitHubActions;


    [GitRepository] [Required] readonly GitRepository GitRepository;
    [Parameter("Ignore unreachable sources during " + nameof(Restore))] readonly bool IgnoreFailedSources;

    [Parameter("Nuget API key")] [Secret] readonly string NuGetApiKey;

    [MinVer] [Required] readonly MinVer MinVer;


    [Solution(GenerateProjects = true)] readonly Solution Solution;
    public IEnumerable<Project> TestProjects => [Solution.Unit];


    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";
    AbsolutePath PackagesDirectory => ArtifactsDirectory / "packages";

    AbsolutePath ReportDirectory => ArtifactsDirectory / "reports";
    AbsolutePath TestResultDirectory => ArtifactsDirectory / "test-results";


    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution)
                .SetIgnoreFailedSources(IgnoreFailedSources)
            );
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .WhenSkipped(DependencyBehavior.Skip)
        .Executes(() =>
        {
            ReportSummary(_ => _
                .AddPair("Version", MinVer.MinVerVersion)
            );

            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .When(IsServerBuild, _ => _
                    .EnableContinuousIntegrationBuild()
                    .SetDeterministic(true))
                .SetAssemblyVersion(MinVer.AssemblyVersion)
                .SetFileVersion(MinVer.FileVersion)
                .SetInformationalVersion(MinVer.MinVerVersion)
            );
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(TestResultDirectory / "*.trx")
        .Produces(TestResultDirectory / "*.xml")
        .Executes(() =>
        {
            try
            {
                DotNetTest(_ => _
                        .SetConfiguration(Configuration)
                        .SetNoBuild(SucceededTargets.Contains(Compile))
                        .ResetVerbosity()
                        .SetResultsDirectory(TestResultDirectory)
                        .CombineWith(TestProjects, (_, v) => _
                            .SetProjectFile(v)
                            .AddLoggers($"trx;LogFileName={v.Name}.trx")
                        ),
                    completeOnFailure: true,
                    degreeOfParallelism: DegreeOfParallelism);
            }
            finally
            {
                ReportTestCount();
            }

            void ReportTestCount()
            {
                IEnumerable<string> GetOutcomes(AbsolutePath file)
                    => XmlTasks.XmlPeek(
                        file,
                        "/xn:TestRun/xn:Results/xn:UnitTestResult/@outcome",
                        ("xn", "http://microsoft.com/schemas/VisualStudio/TeamTest/2010"));

                var resultFiles = TestResultDirectory.GlobFiles("*.trx");
                var outcomes = resultFiles.SelectMany(GetOutcomes).ToList();
                var passedTests = outcomes.Count(x => x == "Passed");
                var failedTests = outcomes.Count(x => x == "Failed");
                var skippedTests = outcomes.Count(x => x == "NotExecuted");

                ReportSummary(_ => _
                    .When(failedTests > 0, _ => _
                        .AddPair("Failed", failedTests.ToString()))
                    .AddPair("Passed", passedTests.ToString())
                    .When(skippedTests > 0, _ => _
                        .AddPair("Skipped", skippedTests.ToString())));
            }
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .Produces(PackagesDirectory / "*.nupkg")
        .Executes(() =>
        {
            var publishCombinations =
                from project in new[] { Solution.JsonConsoleFormatters }
                from framework in project.GetTargetFrameworks()
                select new { project, framework };


            DotNetPublish(_ => _
                    .SetConfiguration(Configuration)
                    .When(IsServerBuild, _ => _
                        .EnableContinuousIntegrationBuild())
                    .SetAssemblyVersion(MinVer.AssemblyVersion)
                    .SetFileVersion(MinVer.FileVersion)
                    .SetInformationalVersion(MinVer.MinVerVersion)
                    .CombineWith(publishCombinations, (_, v) => _
                        .SetProject(v.project)
                        .SetFramework(v.framework)
                    )
                , DegreeOfParallelism);

            DotNetPack(_ => _
                    .SetProject(Solution)
                    .SetConfiguration(Configuration)
                    .SetNoBuild(SucceededTargets.Contains(Compile))
                    .SetOutputDirectory(PackagesDirectory)
                    .SetRepositoryUrl(GitRepository.HttpsUrl)
                    .SetVersion(MinVer.PackageVersion)
                // .WhenNotNull(this as IHazChangelog, (_, o) => _
                //     .SetPackageReleaseNotes(o.NuGetReleaseNotes))
            );

            ReportSummary(_ => _
                .AddPair("Packages", PackagesDirectory.GlobFiles("*.nupkg").Count.ToString()));
        });

    Target Push => _ => _
        .DependsOn(Pack)
        // .Requires(() => GitHubActions)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Requires(() => NuGetApiKey)
        .Executes(() =>
        {
            var packages = PackagesDirectory.GlobFiles("JsonConsoleFormatters*.nupkg");

            Assert.NotEmpty(packages);

            DotNetNuGetPush(_ => _
                .SetSource("https://api.nuget.org/v3/index.json")
                .SetApiKey(NuGetApiKey)
                .SetSkipDuplicate(true)
                .CombineWith(
                    packages, (_, v) => _
                        .SetTargetPath(v)
                )
            );
        });
    
    
    public static int Main() => Execute<Build>(x => x.Pack);
}
