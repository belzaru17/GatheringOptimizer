using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;

namespace GatheringOptimizer.Algorithm;

internal record GatheringResult(double Min, double Avg, double Max, ImmutableArray<IAction> Actions, GatheringState State)
{
    public GatheringResult ExecuteAction(IAction action)
    {
        ActionResult actionResult = action.Execute(State);

        return new GatheringResult(Min + actionResult.Min, Avg + actionResult.Avg, Max + actionResult.Max,
            [.. Actions.ToArray().Append(action)], actionResult.NewGatheringState);
    }

    public static bool BetterMin(GatheringResult value, GatheringResult best)
    {
        return value.Min > best.Min;
    }

    public static bool BetterAvg(GatheringResult value, GatheringResult best)
    {
        return value.Avg > best.Avg;
    }

    public static bool BetterMax(GatheringResult value, GatheringResult best)
    {
        return value.Max > best.Max;
    }
}