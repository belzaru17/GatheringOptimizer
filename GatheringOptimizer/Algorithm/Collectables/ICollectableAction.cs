namespace GatheringOptimizer.Algorithm.Collectables;

internal interface ICollectableAction : IAction
{
    CollectableBuff? Buff { get; }
}

internal record CollectableBuff(uint StatusId, uint StatusIconId);
