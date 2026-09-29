using HarmonyLib;
using MiraAPI.Modifiers;
using JackOfAllMods.Modifiers.Hidden;

namespace JackOfAllMods.Patches;

[HarmonyPatch(typeof(LogicOptions), nameof(LogicOptions.GetPlayerSpeedMod))]
public static class NullsPlayerSpeedPatch
{
    public static void Postfix(PlayerControl pc, ref float __result)
    {
        if (pc.TryGetModifier<ShackledDragModifier>(out var shackled))
        {
            __result *= shackled.SpeedFactor;
        }
    }
}
