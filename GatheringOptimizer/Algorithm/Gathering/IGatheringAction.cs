namespace GatheringOptimizer.Algorithm.Gathering;

internal interface IGatheringAction : IAction
{
    bool CanExecute(GatheringState state);

    ActionResult Execute(GatheringState state);
}