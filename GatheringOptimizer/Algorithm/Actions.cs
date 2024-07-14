using System;

namespace GatheringOptimizer.Algorithm;

internal abstract class BaseAction : IAction
{
    public abstract string Name_MINER { get; }
    public abstract string Name_BOTANIST { get; }

    public abstract int GP { get; }

    public virtual double GatheringBonus => 0;

    public virtual double GatherersBoonBonus => 0;
    public virtual int GatherersBoonExtraItems => 0;

    public virtual int ExtraAttempts => 0;
    public virtual int ExtraAttemptsProcs => 0;

    public virtual int AttemptExtraItems => 0;

    public virtual int ExtraBountifulYieldCount => 0;
}

internal sealed class IncreaseGatheringChance : BaseAction
{
    public override string Name_MINER => "Sharp Vision";
    public override string Name_BOTANIST => "Field Mastery";
    public override int GP => 50;
    public override double GatheringBonus => 0.05;

    public static IncreaseGatheringChance Instance => instance.Value;

    private static readonly Lazy<IncreaseGatheringChance> instance = new(() => new IncreaseGatheringChance());

    private IncreaseGatheringChance() { }
}

internal sealed class IncreaseGatheringChanceII : BaseAction
{
    public override string Name_MINER => "Sharp Vision II";
    public override string Name_BOTANIST => "Field Mastery II";
    public override int GP => 100;
    public override double GatheringBonus => 0.10;

    public static IncreaseGatheringChanceII Instance => instance.Value;

    private static readonly Lazy<IncreaseGatheringChanceII> instance = new(() => new IncreaseGatheringChanceII());

    private IncreaseGatheringChanceII() { }
}

internal sealed class IncreaseGatheringChanceIII : BaseAction
{
    public override string Name_MINER => "Sharp Vision III";
    public override string Name_BOTANIST => "Field Mastery III";
    public override int GP => 250;
    public override double GatheringBonus => 0.50;

    public static IncreaseGatheringChanceIII Instance => instance.Value;

    private static readonly Lazy<IncreaseGatheringChanceIII> instance = new(() => new IncreaseGatheringChanceIII());

    private IncreaseGatheringChanceIII() { }
}

internal sealed class IncreaseBoonChanceI : BaseAction
{
    public override string Name_MINER => "Mountaineer's Gift I";
    public override string Name_BOTANIST => "Pioneer's Gift I";
    public override int GP => 50;
    public override double GatherersBoonBonus => 0.10;

    public static IncreaseBoonChanceI Instance => instance.Value;

    private static readonly Lazy<IncreaseBoonChanceI> instance = new(() => new IncreaseBoonChanceI());

    private IncreaseBoonChanceI() { }
}

internal sealed class IncreaseBoonChanceII : BaseAction
{
    public override string Name_MINER => "Mountaineer's Gift II";
    public override string Name_BOTANIST => "Pioneer's Gift II";
    public override int GP => 100;
    public override double GatheringBonus => 0.30;

    public static IncreaseBoonChanceII Instance => instance.Value;

    private static readonly Lazy<IncreaseBoonChanceII> instance = new(() => new IncreaseBoonChanceII());

    private IncreaseBoonChanceII() { }
}

internal sealed class IncreaseBoonChanceBoth : BaseAction
{
    public override string Name_MINER => "Mountaineer's Gift I + Mountaineer's Gift II";
    public override string Name_BOTANIST => "Pioneer's Gift I + Pioneer's Gift II";
    public override int GP => 150;
    public override double GatheringBonus => 0.40;

    public static IncreaseBoonChanceBoth Instance => instance.Value;

    private static readonly Lazy<IncreaseBoonChanceBoth> instance = new(() => new IncreaseBoonChanceBoth());

    private IncreaseBoonChanceBoth() { }
}

internal sealed class IncreaseBoonItems : BaseAction
{
    public override string Name_MINER => "Nald'thal's Tidings";
    public override string Name_BOTANIST => "Nophica's Tidings";
    public override int GP => 200;
    public override int GatherersBoonExtraItems => 1;

    public static IncreaseBoonItems Instance => instance.Value;

    private static readonly Lazy<IncreaseBoonItems> instance = new(() => new IncreaseBoonItems());

    private IncreaseBoonItems() { }
}

internal sealed class IncreaseAttemptItems : BaseAction
{
    public override string Name_MINER => "Kings Yield";
    public override string Name_BOTANIST => "Blessed Harvest";
    public override int GP => 400;
    public override int AttemptExtraItems => 1;

    public static IncreaseAttemptItems Instance => instance.Value;

    private static readonly Lazy<IncreaseAttemptItems> instance = new(() => new IncreaseAttemptItems());

    private IncreaseAttemptItems() { }
}

internal sealed class IncreaseAttemptItemsII : BaseAction
{
    public override string Name_MINER => "Kings Yield II";
    public override string Name_BOTANIST => "Blessed Harvest II";
    public override int GP => 500;
    public override int AttemptExtraItems => 2;

    public static IncreaseAttemptItemsII Instance => instance.Value;

    private static readonly Lazy<IncreaseAttemptItemsII> instance = new(() => new IncreaseAttemptItemsII());

    private IncreaseAttemptItemsII() { }
}

internal sealed class IncreaseAttempts : BaseAction
{
    public override string Name_MINER => "Solid Reason";
    public override string Name_BOTANIST => "Ageless Words";
    public override int GP => 300;
    public override int ExtraAttempts => 1;
    public override int ExtraAttemptsProcs => 1;

    public static IncreaseAttempts Instance => instance.Value;

    private static readonly Lazy<IncreaseAttempts> instance = new(() => new IncreaseAttempts());

    private IncreaseAttempts() { }
}

internal sealed class IncreaseNextAttemptItems : BaseAction
{
    public override string Name_MINER => "Bountiful Yield";
    public override string Name_BOTANIST => "Bountiful Harvest";
    public override int GP => 100;
    public override int ExtraBountifulYieldCount => 1;

    public static IncreaseNextAttemptItems Instance => instance.Value;

    private static readonly Lazy<IncreaseNextAttemptItems> instance = new(() => new IncreaseNextAttemptItems());

    private IncreaseNextAttemptItems() { }
}

// Clear Vision / Flora Mastery