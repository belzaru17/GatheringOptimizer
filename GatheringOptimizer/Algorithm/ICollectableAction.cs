namespace GatheringOptimizer.Algorithm;

internal interface ICollectableAction : IAction
{
    int ActionId_MINER { get; }
    int ActionId_BOTANIST { get; }

    CollectableBuff? Buff { get; }
}

internal record CollectableBuff(uint StatusId, uint StatusIconId);
