using JAM;
using JAM.Assets;
using JAM.Options.Roles.Impostor;
using JAM.Roles.Impostor;
using MiraAPI.GameOptions;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Assets;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modules.MedSpirit;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles.Crewmate;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.Buttons.Crewmate;

public sealed class ProjectButton : TownOfUsRoleButton<ProjectorRole>
{
    public override string Name => "PROJECT";
    public override BaseKeybind Keybind => Keybinds.SecondaryAction;
    public override Color TextOutlineColor => TownOfUsColors.Impostor;
    public override float Cooldown => Math.Clamp(OptionGroupSingleton<ProjectorOptions>.Instance.ProjectCd + MapCooldown, 0.001f, 120f);
    public override float EffectDuration => OptionGroupSingleton<ProjectorOptions>.Instance.ProjectDuration;

    public override LoadableAsset<Sprite> Sprite => JamRoleIcons.Projector;

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
        if (EffectActive)
        {
            if(Role.Spirit == null)
            {
                Role.Spirit = GameObject.FindObjectsByType<MedSpiritObject>(FindObjectsSortMode.None).FirstOrDefault(med => med.AmOwner);
            }

            if (Role.Spirit != null)
            {
                Info("Removing Spirit");
                Role.isProjected = false;
                ProjectorRole.RpcRemoveMediumSpirit(PlayerControl.LocalPlayer, Role.Spirit);
            }
            else Error("Projection Is Null");
            return;
        }
        ProjectorRole.RpcMediate(PlayerControl.LocalPlayer);
    }

    public override void OnEffectEnd()
    {
        if (Role.Spirit == null)
        {
            Error("Projector Spirit Is Null");
            return;
        }
        Role.isProjected = false;
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

        return ((Timer <= 0 && !EffectActive) || (EffectActive && Timer <= EffectDuration - 2.5f - 1f));
    }

    protected override void FixedUpdate(PlayerControl playerControl)
    {
        Role.Spirit = GameObject.FindObjectsByType<MedSpiritObject>(FindObjectsSortMode.None).FirstOrDefault(med => med.AmOwner);
        Role.isProjected = Role.Spirit != null;

        if (EffectActive && !Role.isProjected)
        {
            ResetCooldownAndOrEffect();
        }


        EffectActive = Role.isProjected;

        base.FixedUpdate(playerControl);
    }
}