namespace GatheringOptimizer.Algorithm;

internal interface ICollectableAction : IAction
{
    CollectableBuff? Buff { get; }
}

internal record CollectableBuff(uint StatusId, uint StatusIconId);
