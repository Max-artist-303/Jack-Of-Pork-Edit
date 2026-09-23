using Il2CppSystem.Web.Util;
using K2AmongUs.Modifiers;
using K2AmongUs.Modifiers.Game.Universal;
using K2AmongUs.Options.Roles.Neutral;
using K2AmongUs.Roles.Neutral;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Networking;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modules;
using TownOfUs.Utilities;
using UnityEngine;

namespace K2AmongUs.Buttons.Game.Universal;

public sealed class BountyHuntingButton : TownOfUsKillRoleButton<RoleBehaviour, PlayerControl>
{
    public override string Name => "HUNT";
    public override Color TextOutlineColor => K2AndNull.Colors.BountyHunter;

    public override float Cooldown => OptionGroupSingleton<BountyHunterOptions>.Instance.HuntedGracePeriod;

    public override LoadableAsset<Sprite> Sprite => K2AmongUs.Assets.K2RoleIcons.BountyHunter;

    public override float Distance => base.Distance / 3f;

    public override bool Enabled(RoleBehaviour? role)
    {
        return MiraAPI.Utilities.Helpers.GetAlivePlayers().Any(p => p.HasModifier<BountyTargetModifier>() && !p.AmOwner && p.Data.Role is not BountyHunterRole && !p.Data.IsDead && p.Data.Role.GetRoleAlignment() != TownOfUs.Roles.RoleAlignment.CrewmateProtective);
    }

    public override bool CanUse()
    {
        return base.CanUse() && Target.HasModifier<BountyTargetModifier>();
    }

    public override PlayerControl? GetTarget()
    {
        return Role.Player.GetClosestLivingPlayer(true, this.Distance);
    }

    protected override void OnClick()
    {
        if (!Target.HasModifier<BountyTargetModifier>())
        {
            Error("Hunting Kill: Target is null");
        }
        else
        {
            PlayerControl.LocalPlayer.RpcCustomMurder(base.Target, MeetingCheck.OutsideMeeting, true, true, true, true, true, true);
            this.Disabled = true;
        }
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        base.FixedUpdate(playerControl);

        if(!MiraAPI.Utilities.Helpers.GetAlivePlayers().Any(p => p.HasModifier<BountyTargetModifier>() && !p.AmOwner && p.Data.Role is not BountyHunterRole && !p.Data.IsDead))
            this.Disabled = true;
    }
}