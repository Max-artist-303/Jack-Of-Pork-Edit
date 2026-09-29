using AmongUs.GameOptions;
using JackOfAllMods.Options.Roles.Neutral;
using JackOfAllMods.Roles.Neutral;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using Reactor.Utilities;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules;
using TownOfUs.Networking;
using TownOfUs.Roles.Crewmate;
using UnityEngine;

namespace JackOfAllMods.Modifiers.Neutral;
public sealed class ZombieRevealedModifier : BaseRevealModifier
{
    public override string ModifierName => "Zombie Reveal";
    public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.Nothing;
    public override bool RevealRole { get; set; } = true;
    public override RoleBehaviour? ShownRole => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ZombieRole>());

    public override void OnDeath(DeathReason reason)
    {
        base.OnDeath(reason);
    }
}
public sealed class ZombieLeaderRevealedModifier() : BaseRevealModifier
{
    public override string ModifierName => "Zombie Leader Revealed";
    public override ChangeRoleResult ChangeRoleResult { get; set; } = ChangeRoleResult.Nothing;
    public override RoleBehaviour ShownRole => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<ZombieLeaderRole>());

    public override bool RevealRole => true;
    public override bool Visible { get; set; } = false;
    public override string ExtraRoleText => string.Empty;

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        Visible = OptionGroupSingleton<ZombieOptions>.Instance.ZombieShowsRole && PlayerControl.LocalPlayer.HasModifier<ZombieAllianceModifier>();
    }
}

public sealed class  ZombieAllianceModifier : AllianceGameModifier
{
    public override string ModifierName => "Zombie Alliance Modifier";
    public override bool HideOnUi => true;
    public override int GetAssignmentChance()
    {
        return 0;
    }

    public override bool? DidWin(GameOverReason gameOverReason)
    {
        if (Helpers.GetAlivePlayers().Any(p => p.Data.Role is ZombieLeaderRole && !p.Data.IsDead))
        {
            ZombieLeaderRole? leader = Helpers.GetAlivePlayers().First(p => p.Data.Role is ZombieLeaderRole && !p.Data.IsDead).Data.Role as ZombieLeaderRole;
            int numNonZombies = Helpers.GetAlivePlayers().Count(p => !(p.Data.Role is ZombieLeaderRole || p.Data.Role is ZombieRole));
            int numZombies = PlayerControl.AllPlayerControls.ToArray().Count(p => p.Data.Role is ZombieRole || p.Data.Role is ZombieLeaderRole);

            return leader?.WinConditionMet() == true;
        }
        return false;
    }
}

public sealed class ZombieArrowModifier(DeadBody deadBody, Color color) : ArrowDeadBodyModifier(deadBody, color, 0)
{
    public override string ModifierName => "Zombie Arrow";

    // Zombie Leader Arrow
    [RegisterEvent(0)]
    public static void AfterMurderEventHandler(AfterMurderEvent @event)
    {
        if (!CustomRoleUtils.GetActiveRolesOfType<ZombieLeaderRole>().Any())
        {
            return;
        }

        if (!OptionGroupSingleton<ZombieOptions>.Instance.ZombieArrows)
        {
            return;
        }

        Coroutines.Start(CoCreateArrow(@event.Target));

        if(PlayerControl.LocalPlayer.Data.Role is ZombieLeaderRole)
            Coroutines.Start(TownOfUs.Utilities.MiscUtils.CoFlash(DestroyableSingleton<ZombieRole>.Instance.RoleColor));
    }

    private static System.Collections.IEnumerator CoCreateArrow(PlayerControl target)
    {
        var deadBody = UnityEngine.Object.FindObjectsOfType<DeadBody>().FirstOrDefault(x => x.ParentId == target.PlayerId);

        if (deadBody == null)
        {
            yield break;
        }

        foreach (var zombieRole in CustomRoleUtils.GetActiveRolesOfType<ZombieLeaderRole>().Select(x => x.Player))
        {
            if (zombieRole.AmOwner)
            {
                zombieRole.AddModifier<ZombieArrowModifier>(deadBody, JackOfAllMods.Colors.Zombie);
            }
        }
    }
}
