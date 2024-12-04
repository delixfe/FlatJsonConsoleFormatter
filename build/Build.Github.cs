using Nuke.Common.CI.GitHubActions;

[GitHubActions("CI",
    GitHubActionsImage.Ubuntu2404,
    AutoGenerate = false,
    On = [GitHubActionsTrigger.Push, GitHubActionsTrigger.WorkflowDispatch],
    InvokedTargets = [nameof(Restore), nameof(Compile), nameof(Test), nameof(Pack)],
    TimeoutMinutes = 5
    // PublishCondition = "false"
)]
partial class Build;
