
using Nuke.Common.CI.GitHubActions;

[GitHubActions("CI",
    GitHubActionsImage.Ubuntu2404,
    On = [GitHubActionsTrigger.Push, GitHubActionsTrigger.WorkflowDispatch],
    OnPushBranches = ["*"],
    
    InvokedTargets = [nameof(Pack)],
    TimeoutMinutes = 5,
    PublishCondition = "false"
)]
partial class Build;
