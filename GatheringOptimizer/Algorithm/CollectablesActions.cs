using System;

namespace GatheringOptimizer.Algorithm;


internal static class CollectableBuffs
{
    public static readonly CollectableBuff Scrutiny = new(757, 16017);
    public static readonly CollectableBuff CollectorsFocus = new(2668, 16022);
    public static readonly CollectableBuff PrimingTouch = new(3910, 16025);

    public static readonly CollectableBuff CollectorsStandard = new(2418, 16018);
    public static readonly CollectableBuff CollectorsHighStandard = new(3911, 16026);

    public static readonly CollectableBuff Eureka = new(2765, 19019);
}

internal class ScourAction : ICollectableAction
{
    public int GP => 0;
    public int ActionId_MINER => 0;
    public int ActionId_BOTANIST => 0;
    public CollectableBuff? Buff => null;

    public string Name_MINER => "Scour";
    public string Name_BOTANIST => Name_MINER;

    public uint IconId_MINER => 1901;
    public uint IconId_BOTANIST => IconId_MINER;


    public static ScourAction Instance => instance.Value;

    private static readonly Lazy<ScourAction> instance = new(() => new ScourAction());

    private ScourAction() { }
}

internal class BrazenProspectorAction : ICollectableAction
{
    public int GP => 0;
    public int ActionId_MINER => 0;
    public int ActionId_BOTANIST => 0;
    public CollectableBuff? Buff => null;

    public string Name_MINER => "Brazen Prospector";
    public string Name_BOTANIST => Name_MINER;

    public uint IconId_MINER => 1902;
    public uint IconId_BOTANIST => IconId_MINER;


    public static BrazenProspectorAction Instance => instance.Value;

    private static readonly Lazy<BrazenProspectorAction> instance = new(() => new BrazenProspectorAction());

    private BrazenProspectorAction() { }
}

internal class MeticulousProspectorAction : ICollectableAction
{
    public int GP => 0;
    public int ActionId_MINER => 0;
    public int ActionId_BOTANIST => 0;
    public CollectableBuff? Buff => null;

    public string Name_MINER => "Meticulous Prospector";
    public string Name_BOTANIST => Name_MINER;

    public uint IconId_MINER => 1903;
    public uint IconId_BOTANIST => IconId_MINER;


    public static MeticulousProspectorAction Instance => instance.Value;

    private static readonly Lazy<MeticulousProspectorAction> instance = new(() => new MeticulousProspectorAction());

    private MeticulousProspectorAction() { }
}

internal class CollectAction : ICollectableAction
{
    public int GP => 0;
    public int ActionId_MINER => 0;
    public int ActionId_BOTANIST => 0;
    public CollectableBuff? Buff => null;

    public string Name_MINER => "Collect";
    public string Name_BOTANIST => Name_MINER;

    public uint IconId_MINER => 1044;
    public uint IconId_BOTANIST => 1094;


    public static CollectAction Instance => instance.Value;

    private static readonly Lazy<CollectAction> instance = new(() => new CollectAction());

    private CollectAction() { }
}

internal class ScrutinyAction : ICollectableAction
{
    public int GP => 200;
    public int ActionId_MINER => 22185;
    public int ActionId_BOTANIST => 22189;
    public CollectableBuff? Buff => CollectableBuffs.Scrutiny;

    public string Name_MINER => "Scrutiny";
    public string Name_BOTANIST => Name_MINER;

    public uint IconId_MINER => 1904;
    public uint IconId_BOTANIST => IconId_MINER;


    public static ScrutinyAction Instance => instance.Value;

    private static readonly Lazy<ScrutinyAction> instance = new(() => new ScrutinyAction());

    private ScrutinyAction() { }
}

internal class CollectorsFocusAction : ICollectableAction
{
    public int GP => 100;
    public int ActionId_MINER => 21205;
    public int ActionId_BOTANIST => 21206;
    public CollectableBuff? Buff => CollectableBuffs.CollectorsFocus;

    public string Name_MINER => "Collector's Focus";
    public string Name_BOTANIST => Name_MINER;

    public uint IconId_MINER => 1948;
    public uint IconId_BOTANIST => 1098;


    public static CollectorsFocusAction Instance => instance.Value;

    private static readonly Lazy<CollectorsFocusAction> instance = new(() => new CollectorsFocusAction());

    private CollectorsFocusAction() { }
}

internal class PrimingTouchAction : ICollectableAction
{
    public int GP => 100;
    public int ActionId_MINER => 34871;
    public int ActionId_BOTANIST => 34872;
    public CollectableBuff? Buff => CollectableBuffs.PrimingTouch;

    public string Name_MINER => "Priming Touch";
    public string Name_BOTANIST => Name_MINER;

    public uint IconId_MINER => 1905;
    public uint IconId_BOTANIST => IconId_MINER;


    public static PrimingTouchAction Instance => instance.Value;

    private static readonly Lazy<PrimingTouchAction> instance = new(() => new PrimingTouchAction());

    private PrimingTouchAction() { }
}

internal class IncreaseCollectableIntegrityAction : ICollectableAction
{
    public int GP => 300;
    public int ActionId_MINER => 232;
    public int ActionId_BOTANIST => 215;
    public CollectableBuff? Buff => CollectableBuffs.Eureka;

    public string Name_MINER => "Solid Reason";
    public string Name_BOTANIST => "Ageless Words";

    public uint IconId_MINER => 1009;
    public uint IconId_BOTANIST => 1059;

    public static IncreaseCollectableIntegrityAction Instance => instance.Value;

    private static readonly Lazy<IncreaseCollectableIntegrityAction> instance = new(() => new IncreaseCollectableIntegrityAction());

    private IncreaseCollectableIntegrityAction() { }
}

internal class CollectableEurekaAction : ICollectableAction
{
    public int GP => 0;
    public int ActionId_MINER => 26521;
    public int ActionId_BOTANIST => 26522;
    public CollectableBuff? Buff => CollectableBuffs.Eureka;

    public string Name_MINER => "Wise to the World";
    public string Name_BOTANIST => "Wise to the World";

    public uint IconId_MINER => 1049;
    public uint IconId_BOTANIST => 1099;

    public static CollectableEurekaAction Instance => instance.Value;

    private static readonly Lazy<CollectableEurekaAction> instance = new(() => new CollectableEurekaAction());

    private CollectableEurekaAction() { }
}