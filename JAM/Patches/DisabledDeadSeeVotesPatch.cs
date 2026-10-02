using HarmonyLib;
using TownOfUs.Patches.Options;

namespace JAM.Patches;

[HarmonyPatch(typeof(DeadSeeVoteColorsPatch), nameof(DeadSeeVoteColorsPatch.Prefix))]
public static class DisableDeadSeeVotesPatch
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        return false;
    }
}
