using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatheringOptimizer.Algorithm;

internal interface IBuff
{
    string DebugName { get; }

    bool Ephemeral { get; }

    double GatheringBonus { get; }

    double GatherersBoonBonus { get; }
    int GatherersBoonExtraItems { get; }

    bool ExtraAttemptProc { get; }

    int ExtraYield { get; }
    bool BountifulYield { get; }
}
