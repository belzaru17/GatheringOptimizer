namespace GatheringOptimizer.Algorithm;

internal interface IAction
{
    int GP { get; }

    string Name_MINER { get; }
    string Name_BOTANIST { get; }

    uint IconId_MINER { get; }
    uint IconId_BOTANIST { get; }

    int ActionId_MINER { get; }
    int ActionId_BOTANIST { get; }
}