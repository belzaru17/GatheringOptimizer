namespace GatheringOptimizer.Algorithm;

internal interface IAction
{
    string Name_MINER { get; }
    string Name_BOTANIST { get; }

    uint IconId_MINER { get; }
    uint IconId_BOTANIST { get; }

    int GP { get; }

    bool CanExecute(GatheringState state);

    ActionResult Execute(GatheringState state);
}