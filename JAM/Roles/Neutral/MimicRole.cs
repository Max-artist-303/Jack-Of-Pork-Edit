using AmongUs.GameOptions;
using Hazel;
using JAM.Assets;
using JAM.Options.OptionTypes;
using JAM.Options.Roles.Neutral;
using MiraAPI.Events;
using MiraAPI.Events.Vanilla.Gameplay;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Interfaces;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Roles.Neutral;
using TownOfUs.Utilities;
using UnityEngine;

namespace JAM.Roles.Neutral;

public sealed class MimicRole(IntPtr cppPtr) : NeutralRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public string RoleName => "Mimic";
    public RoleAlignment RoleAlignment => RoleAlignment.NeutralKilling;
    public Color RoleColor => Colors.Mimic;
    public ModdedRoleTeams Team => ModdedRoleTeams.Custom;
    public DoomableType DoomHintType => DoomableType.Trickster;
    public string RoleDescription => "Copy-Pasta!";
    public string RoleMedDescription => "Each Round, Pick A Role To Mimic For That Round.";
    public string RoleLongDescription => "Each Round, Pick A Role To Mimic For That Round.";
    public string GetAdvancedDescription() { return RoleLongDescription + TownOfUs.Utilities.MiscUtils.AppendOptionsText(base.GetType()); }

    public CustomRoleConfiguration Configuration => new(this)
    {
        IntroSound = TouAudio.DetectiveIntroSound,
        Icon = JamRoleIcons.Mimic,
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(JamRoleIcons.Mimic.LoadAsset(), "JackOfAllMods.Roles.Neutral.Mimic", 1.45f),
        GhostRole = (RoleTypes)RoleId.Get<NeutralGhostRole>()
    };

    public static List<RoleBehaviour> GetValidMimicRoles()
    {
        List<RoleBehaviour> roleList = DestroyableSingleton<RoleManager>.Instance.AllRoles.ToArray().Where(delegate (RoleBehaviour r)
        {
            if (OptionGroupSingleton<MimicOptions>.Instance.BlacklistedRoles.rolesList.Any(role => role.Role == r.Role))
                return false;

            return IsValidMimicRole(r);
        }).ToList();

        return roleList;
    }

    public static bool IsValidMimicRole(RoleBehaviour role)
    {
        RoleManager.RoleAssignmentData roleData = CustomRoleUtils.GetAssignData(role.Role);

        if (roleData.Count == 0 || roleData.Chance == 0) return false; // Only If It Can Currenlty Be In The Game
        if (!CustomRoleUtils.CanSpawnOnCurrentMode(role)) return false; // Only If It Can Spawn On The Current Mode
        if (role is IGhostRole) return false; // No Ghost Roles
        if (role is MimicRole) return false; // Not Self
        if (role is IAssignableTargets) return false; // Not A Targeting One (eg. Exe or Fairy)
        if (role is MercenaryRole) return false; // Technically Not A Targeting One
        if (role is IDoubleDraftRole) return false; // For Things Like Recruiter And Vampire
        if (role is ZombieLeaderRole) return false; // I... don't want to deal with the consequences of this...

        if (role.IsImpostor()) return false; // Stupid Fix Right Now So Mimic At Least Can Be Played

        if (role.GetRoleAlignment() == RoleAlignment.NeutralOutlier) return false; // Removing Chef, Inquiz, etc.
        if (role.GetRoleAlignment() == RoleAlignment.CrewmatePower) return false; // No Crewmate Power
        if (role.GetRoleAlignment() == RoleAlignment.CrewmateSupport) return false; // No Crewmate Support

        return true; //Will Be Ok Here
    }

    // Events And Such
    [RegisterEvent(0)]
    public static void OnRoundStartEvent(RoundStartEvent @event)
    {
        if (@event.TriggeredByIntro && PlayerControl.LocalPlayer.IsHost())
        {
            RoleTypes[] roleTypes = OptionGroupSingleton<MimicOptions>.Instance.BlacklistedRoles.rolesList.Select(r => r.Role).ToArray();

            Rpc<SyncMimicBlacklistRpc>.Instance.Send(roleTypes);
        }
    }
}

[RegisterCustomRpc((uint)JAMRpcCalls.SyncMimicBlacklist)]
public class SyncMimicBlacklistRpc(Plugin plugin, uint id) : PlayerCustomRpc<JAM.Plugin, RoleTypes[]>(plugin, id)
{
    public override RpcLocalHandling LocalHandling => RpcLocalHandling.After;

    public override void Handle(PlayerControl sender, RoleTypes[]? data)
    {
        OptionGroupSingleton<MimicOptions>.Instance.BlacklistedRoles.rolesList.RemoveAll(r => true);

        foreach (RoleTypes roleType in data)
        {
            OptionGroupSingleton<MimicOptions>.Instance.BlacklistedRoles.rolesList.Add(DestroyableSingleton<RoleManager>.Instance.GetRole(roleType));
        }
    }

    public override RoleTypes[] Read(MessageReader reader)
    {
        int length = reader.ReadInt32();
        RoleTypes[] roleTypes = new RoleTypes[length];

        for(int i = 0; i < length; i++)
        {
            roleTypes[i] = (RoleTypes)reader.ReadUInt32();
            if (roleTypes[i] == null)
            {
                Error("Error: Role Not Found - " + reader.ReadUInt32());
            }
            else
            {
                Info("New Blacklist Role: " + roleTypes[i]);
            }
        }

        return roleTypes;
    }

    public override void Write(MessageWriter writer, RoleTypes[]? data)
    {
        writer.Write(data.Length);

        foreach(var value in data)
        {
            Info("Sending Blacklist Role: " + value.ToString());
            writer.Write((uint)value);
        }
    }
}
