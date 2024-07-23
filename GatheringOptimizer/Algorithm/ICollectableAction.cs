using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatheringOptimizer.Algorithm;

internal interface ICollectableAction : IAction
{
    CollectableBuff? Buff { get; }
}

internal record CollectableBuff(uint StatusId, uint StatusIconId);
