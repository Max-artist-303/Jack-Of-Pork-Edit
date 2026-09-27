using MiraAPI.Modifiers;
using NullsMod.Modifiers.Hidden;
using PerfectComms.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TownOfUs.Utilities;

namespace NullsMod.CommsPatches;

public static class CamouflagerComms
{
    public static VoiceRuleResult CamouflagerVoiceRule(VoiceRuleContext ctx)
    {
        if (PlayerControl.LocalPlayer == null || ctx.Player == null) return VoiceRuleResult.Pass;

        if (!PlayerControl.LocalPlayer.Data.IsDead && ctx.Player.HasModifier<CamouflagerCamoModifier>())
        {
            return VoiceRuleResult.Mute("Silenced By Comms");
        }

        return VoiceRuleResult.Pass;
    }
}
