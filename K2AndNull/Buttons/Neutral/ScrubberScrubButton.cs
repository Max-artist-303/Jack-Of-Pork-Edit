using JackOfAllMods.Assets;
using JackOfAllMods.Modifiers.Neutral;
using JackOfAllMods.Options.Roles.Neutral;
using JackOfAllMods.Roles.Neutral;
using MiraAPI.GameOptions;
using MiraAPI.Hud;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Utilities;
using UnityEngine;

namespace JackOfAllMods.Buttons.Crewmate;

public sealed class ScrubberScrubButton : TownOfUsRoleButton<ScrubberRole, PlayerControl>
{
    PlayerControl scrubbedPlayer;

    public override string Name => "SCRUB";
    public override BaseKeybind Keybind => Keybinds.PrimaryAction;
    public override Color TextOutlineColor => JackOfAllMods.Colors.Scrubber;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<ScrubberOptions>.Instance.ScrubCooldown, 5f, 120f);
    public override LoadableAsset<Sprite> Sprite => K2RoleIcons.Scrubber;

    public override void CreateButton(Transform parent)
    {
        base.CreateButton(parent);
        Coroutines.Start(MiscUtils.CoMoveButtonIndex(this, false));
    }

    public override PlayerControl? GetTarget()
    {
        return PlayerControl.LocalPlayer.GetClosestLivingPlayer(true, Distance);
    }
	
    public override bool CanUse()
    {
        return base.CanUse() && Target.HasModifier<BaseModifier>();
    }

    protected override void OnClick()
    {
        if (Target == null)
        {
            Error("Cleanser Cleanse: Target is null");
            return;
        }
        scrubbedPlayer = Target;
    }

    public override void OnEffectEnd()
    {
        base.OnEffectEnd();

        if (Role.Player.AmOwner)
        {
            ScrubberRole.RpcScrubModifiers(Role.Player, scrubbedPlayer);
            ResetCooldownAndOrEffect();
        }
    }
    public override float EffectDuration => OptionGroupSingleton<ScrubberOptions>.Instance.ScrubDelay;
}
