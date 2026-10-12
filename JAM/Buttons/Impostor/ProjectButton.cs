using JAM.Assets;
using JAM.Modifiers.Hidden;
using JAM.Options.Roles.Impostor;
using JAM.Roles.Impostor;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Modifiers;
using TownOfUs.Modules;
using TownOfUs.Modules.Components;
using TownOfUs.Options.Maps;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace JAM.Buttons.Impostor;

public sealed class ProjectButton : TownOfUsRoleButton<ProjectorRole>, ILegacyCapable
{
    public override string Name => "Project";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Medium;
    public override LoadableAsset<Sprite> Sprite => LegacyAssets.IsLegacy ? JamAssets.ProjectButton : JamAssets.ProjectButton;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<ProjectorOptions>.Instance.ProjectCd + MapCooldown, 0.001f, 120f);
    public override float EffectDuration => OptionGroupSingleton<ProjectorOptions>.Instance.ProjectDuration;

    public override bool ZeroIsInfinite { get; set; } = true;

    public override void ClickHandler()
    {
        if (!CanUse())
        {
            return;
        }

        OnClick();
        Button?.SetDisabled();
        if (EffectActive)
        {
            Timer = Cooldown;
            EffectActive = false;
        }
        else if (HasEffect)
        {
            EffectActive = true;
            Timer = EffectDuration;
        }
        else
        {
            Timer = Cooldown;
        }
    }

    protected override void OnClick()
    {
        if (Role.Spirit != null)
        {
            ResetCooldownAndOrEffect();
            if (Role.Spirit != null)
            {
                ProjectorRole.RpcRemoveMediumSpirit(PlayerControl.LocalPlayer, Role.Spirit);
            }
            else Info("No Spirit Found");
            return;
        }

        ProjectorRole.RpcMediate(PlayerControl.LocalPlayer);
    }

    public override void OnEffectEnd()
    {
        if (Role.Spirit == null)
        {
            Info("No Spirit");
            return;
        }
        Info("Removing Spirit");
        ProjectorRole.RpcRemoveMediumSpirit(PlayerControl.LocalPlayer, Role.Spirit);
    }

    public override bool CanUse()
    {
        if (HudManager.Instance.Chat.IsOpenOrOpening || MeetingHud.Instance)
        {
            return false;
        }

        if (PlayerControl.LocalPlayer.GetModifiers<DisabledModifier>().Any(x => !x.CanUseAbilities))
        {
            return false;
        }

        return EffectActive || Timer <= 0;
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        EffectActive = Role.isProjected;

        base.FixedUpdate(playerControl);
    }
}
