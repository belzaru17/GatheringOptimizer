using FFXIVClientStructs.FFXIV.Client.System.String;
using FFXIVClientStructs.FFXIV.Component.GUI;

namespace GatheringOptimizer.Windows;

internal static class AddonUtils
{
    public delegate void TextNodeAction(Utf8String textNode);

    public static unsafe void GetTextNode(AtkComponentCheckBox* component, uint id, TextNodeAction action)
    {
        if (component == null) return;
        var text = GetTextNode(component->GetTextNodeById(id));
        if (text.HasValue)
        {
            action(text.Value);
        }
    }

    public static unsafe Utf8String? GetTextNode(AtkComponentCheckBox* component, uint id)
    {
        return (component == null) ? null : GetTextNode(component->GetTextNodeById(id));
    }

    public static unsafe Utf8String? GetTextNode(AtkResNode* node)
    {
        return (node == null) ? null : GetTextNode(node->GetAsAtkTextNode());
    }

    public static unsafe Utf8String? GetTextNode(AtkTextNode* textNode)
    {
        return (textNode == null) ? null : textNode->NodeText;
    }

    public static bool IsBotanist()
    {
        return Plugin.ClientState.LocalPlayer?.ClassJob.Id == 17;
    }
}
