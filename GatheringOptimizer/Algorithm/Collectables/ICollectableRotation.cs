using System;

namespace GatheringOptimizer.Algorithm.Collectables;

internal interface ICollectableRotation
{
    string Title { get; }
    int MinGP { get; }
    bool Leveling { get; }

    Tuple<int, ICollectableAction> NextAction(int step, int currentGP, int integrity, int maxIntegrity, int collectability, CollectableBuff? buff, bool eurekaBuff);
}