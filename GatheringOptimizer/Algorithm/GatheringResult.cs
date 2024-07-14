using System.Collections.Immutable;
using System.Linq;

namespace GatheringOptimizer.Algorithm;

internal record GatheringResult(double Min, double Avg, double Max, ImmutableArray<IAction> Actions, GatheringState State)
{
    public GatheringResult ExecuteAction(IAction action)
    {
        ActionResult actionResult = action.Execute(State);

        return new GatheringResult(Min + actionResult.Min, Avg + actionResult.Avg, Max + actionResult.Max,
            [.. Actions.ToArray().Append(action)], actionResult.NewGatheringState);
    }
}