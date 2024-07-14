namespace GatheringOptimizer.Algorithm;

internal interface IAction
{
    string Name_MINER { get; }
    string Name_BOTANIST { get; }

    int GP { get; }

    bool CanExecute(GatheringState state);

    ActionResult Execute(GatheringState state);
}