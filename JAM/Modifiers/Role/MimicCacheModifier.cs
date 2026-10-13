using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using JAM.Assets;
using JAM.Modifiers.Hidden;
using JAM.Options.Roles.Impostor;
using JAM.Roles.Neutral;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.Events.Vanilla.Meeting;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Translation;
using TownOfUs.Assets;
using TownOfUs.Events;
using TownOfUs.Extensions;
using TownOfUs.Interfaces;
using TownOfUs.Modifiers;
using TownOfUs.Modifiers.Crewmate;
using TownOfUs.Modifiers.Game;
using TownOfUs.Modules;
using TownOfUs.Modules.Wiki;
using TownOfUs.Options.Roles.Crewmate;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;

namespace JAM.Modifiers.Role;

public sealed class MimicCacheModifier : BaseModifier, ICachedRole
{
    public override string ModifierName => "Mimic Cache Modifier";
    public override bool HideOnUi => true;

    public bool ShowCurrentRoleFirst => true;

    public bool Visible => Player.AmOwner || PlayerControl.LocalPlayer.HasDied() ||
                           FairyRole.FairySeesRoleVisibilityFlag(Player);

    public CacheRoleGuess GuessMode => CacheRoleGuess.CachedRole;

    public RoleBehaviour CachedRole => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<MimicRole>());
    public string CachedRoleName => "M";

    public override void OnActivate()
    {
        Player.AddModifier<MimicAllianceModifier>();

        base.OnActivate();
    }

    [RegisterEvent(0)]
    public static void OnEjectionEvent(EjectionEvent @event)
    {
        if(PlayerControl.LocalPlayer.HasModifier<MimicCacheModifier>())
        {
            PlayerControl.LocalPlayer.RpcChangeRole(RoleId.Get<MimicRole>(), false);
        }
    }

    //Remove On True Role Change
    [HarmonyPatch(typeof(TownOfUs.Utilities.Extensions), "RpcChangeRole", [typeof(PlayerControl), typeof(ushort), typeof(bool)])]
    public static class TrulyChangeRolePatch
    {
        public static void Prefix(PlayerControl player, bool recordRole)
        {
            if (player.HasModifier<MimicCacheModifier>() && recordRole)
            {
                player.RpcRemoveModifier<MimicCacheModifier>();
            }
        }
    }

    [RegisterEvent(0)]
    public static void OnGameEndEvent(GameEndEvent @event)
    {
        if (PlayerControl.LocalPlayer.HasModifier<MimicCacheModifier>())
        {
            PlayerControl.LocalPlayer.RpcChangeRole(RoleId.Get<MimicRole>(), false);
        }
    }

    [HarmonyPatch(typeof(TownOfUs.Utilities.Extensions), "IsImpostorAligned")]
    public static class MimicAintImp
    {
        public static void Postfix(ref bool __result, ref PlayerControl player)
        {
            if (player.HasModifier<MimicCacheModifier>())
            {
                __result = false;
            }
        }
    }

    [HarmonyPatch(typeof(TownOfUs.Modules.GameHistory), "GetRoleWhenAlive", [typeof(PlayerControl)])]
    public static class MimicHidesFromItsFakeRoles
    {
        public static void Postfix(ref RoleBehaviour __result, PlayerControl player)
        {
            if(player.GetModifier<MimicCacheModifier>() is MimicCacheModifier modifier)
            {
                Info("Mimic Not Tricking Anyone");
                __result = modifier.CachedRole;
            }
        }
    }
    //Override GetRoleWhenAlive for mimic
}

public sealed class MimicAllianceModifier : AllianceGameModifier, IWikiDiscoverable
{
    public override string ModifierName => "Mimic Alliance Modifier";
    public bool IsHiddenFromList => true;
    public override bool GetsPunished => false;

    public override int GetAssignmentChance()
    {
        return 0;
    }
}