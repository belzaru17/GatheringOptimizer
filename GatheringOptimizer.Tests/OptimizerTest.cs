using GatheringOptimizer.Algorithm;
using GatheringOptimizer.Algorithm.Gathering;

namespace GatheringOptimizer.Tests;

[TestFixture]
public class RetainerVentureItemFactoryTest
{
    [TestCase(944, 944, 6, 1.0, 0.3, 1, 2, ExpectedResult = 28.4)]
    public double TestOptimizerAvg(int maxGP, int currentGP, int integrity, double baseGatheringChance, double baseGatherersBoonChance, int attemptItems, int bountifulYieldItems)
    {
        GatheringParameters parameters = new GatheringParameters(maxGP, integrity, baseGatheringChance, baseGatherersBoonChance, attemptItems, bountifulYieldItems);
        var best = Optimizer.GenerateBestResult(parameters, GatheringResult.BetterAvg, currentGP);
        return best.Avg;
    }

    [TestCase(944, 944, 6, 1.0, 0.3, 1, 2, ExpectedResult = "Mountaineer's Gift I,Mountaineer's Gift II,Nald'thal's Tidings,Kings Yield II,Gather,Bountiful Yield,Gather,Gather,Gather,Gather,Gather")]
    public string TestOptimizerActions(int maxGP, int currentGP, int integrity, double baseGatheringChance, double baseGatherersBoonChance, int attemptItems, int bountifulYieldItems)
    {
        GatheringParameters parameters = new GatheringParameters(maxGP, integrity, baseGatheringChance, baseGatherersBoonChance, attemptItems, bountifulYieldItems);
        var best = Optimizer.GenerateBestResult(parameters, GatheringResult.BetterAvg, currentGP);
        var actions = String.Join(",", best.Actions.Select(x => x.Name_MINER));
        return actions;
    }
}