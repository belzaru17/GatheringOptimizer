using System;

namespace GatheringOptimizer.Algorithm.Gathering;

internal abstract class BaseBuff : IBuff
{
    public abstract string DebugName { get; }
    public virtual bool Ephemeral => false;

    public virtual double GatheringBonus => 0;

    public virtual double GatherersBoonBonus => 0;
    public virtual int GatherersBoonExtraItems => 0;

    public virtual bool ExtraAttemptProc => false;

    public virtual int ExtraYield => 0;
    public virtual bool BountifulYield => false;
}

internal sealed class IncreasedGatheringChanceBuff : BaseBuff
{
    public override string DebugName => "Increased Gathering Chance";
    public override double GatheringBonus => 0.05;

    public static IncreasedGatheringChanceBuff Instance => instance.Value;

    private static readonly Lazy<IncreasedGatheringChanceBuff> instance = new(() => new IncreasedGatheringChanceBuff());

    private IncreasedGatheringChanceBuff() { }
}

internal sealed class IncreasedGatheringChanceIIBuff : BaseBuff
{
    public override string DebugName => "Increased Gathering Chance II";
    public override double GatheringBonus => 0.10;

    public static IncreasedGatheringChanceIIBuff Instance => instance.Value;

    private static readonly Lazy<IncreasedGatheringChanceIIBuff> instance = new(() => new IncreasedGatheringChanceIIBuff());

    private IncreasedGatheringChanceIIBuff() { }
}

internal sealed class IncreasedGatheringChanceIIIBuff : BaseBuff
{
    public override string DebugName => "Increased Gathering Chance III";
    public override double GatheringBonus => 0.50;

    public static IncreasedGatheringChanceIIIBuff Instance => instance.Value;

    private static readonly Lazy<IncreasedGatheringChanceIIIBuff> instance = new(() => new IncreasedGatheringChanceIIIBuff());

    private IncreasedGatheringChanceIIIBuff() { }
}

internal sealed class IncreasedBoonChanceIBuff : BaseBuff
{
    public override string DebugName => "Increase Boon Chance I";
    public override double GatherersBoonBonus => 0.10;

    public static IncreasedBoonChanceIBuff Instance => instance.Value;

    private static readonly Lazy<IncreasedBoonChanceIBuff> instance = new(() => new IncreasedBoonChanceIBuff());

    private IncreasedBoonChanceIBuff() { }
}

internal sealed class IncreasedBoonChanceIIBuff : BaseBuff
{
    public override string DebugName => "Increase Boon Chance I";
    public override double GatherersBoonBonus => 0.30;

    public static IncreasedBoonChanceIIBuff Instance => instance.Value;

    private static readonly Lazy<IncreasedBoonChanceIIBuff> instance = new(() => new IncreasedBoonChanceIIBuff());

    private IncreasedBoonChanceIIBuff() { }
}

internal sealed class IncreasedBoonItemsBuff : BaseBuff
{
    public override string DebugName => "Increased Boon Items";
    public override int GatherersBoonExtraItems => 1;

    public static IncreasedBoonItemsBuff Instance => instance.Value;

    private static readonly Lazy<IncreasedBoonItemsBuff> instance = new(() => new IncreasedBoonItemsBuff());

    private IncreasedBoonItemsBuff() { }
}

internal sealed class IncreasedAttemptItemsBuff : BaseBuff
{
    public override string DebugName => "Increased Attempt Items";
    public override int ExtraYield => 1;

    public static IncreasedAttemptItemsBuff Instance => instance.Value;

    private static readonly Lazy<IncreasedAttemptItemsBuff> instance = new(() => new IncreasedAttemptItemsBuff());

    private IncreasedAttemptItemsBuff() { }
}

internal sealed class IncreasedAttemptItemsIIBuff : BaseBuff
{
    public override string DebugName => "Increased Attempt Items II";
    public override int ExtraYield => 2;

    public static IncreasedAttemptItemsIIBuff Instance => instance.Value;

    private static readonly Lazy<IncreasedAttemptItemsIIBuff> instance = new(() => new IncreasedAttemptItemsIIBuff());

    private IncreasedAttemptItemsIIBuff() { }
}

internal sealed class IncreasedNextAttemptItemsBuff : BaseBuff
{
    public override string DebugName => "Increased Next Attempt Items";
    public override bool Ephemeral => true;
    public override bool BountifulYield => true;

    public static IncreasedNextAttemptItemsBuff Instance => instance.Value;

    private static readonly Lazy<IncreasedNextAttemptItemsBuff> instance = new(() => new IncreasedNextAttemptItemsBuff());

    private IncreasedNextAttemptItemsBuff() { }
}

internal sealed class IncreasedNextAttemptGatheringChanceBuff : BaseBuff
{
    public override string DebugName => "Increased Next Attempt Gathering Chance";
    public override bool Ephemeral => true;
    public override double GatheringBonus => 0.15;

    public static IncreasedNextAttemptGatheringChanceBuff Instance => instance.Value;

    private static readonly Lazy<IncreasedNextAttemptGatheringChanceBuff> instance = new(() => new IncreasedNextAttemptGatheringChanceBuff());

    private IncreasedNextAttemptGatheringChanceBuff() { }
}

internal sealed class ExtraAttemptProcBuff : BaseBuff
{
    public override string DebugName => "Extra Attempt Proc";
    public override bool ExtraAttemptProc => true;

    public static ExtraAttemptProcBuff Instance => instance.Value;

    private static readonly Lazy<ExtraAttemptProcBuff> instance = new(() => new ExtraAttemptProcBuff());

    private ExtraAttemptProcBuff() { }
}