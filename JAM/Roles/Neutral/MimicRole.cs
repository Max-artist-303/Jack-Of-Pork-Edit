using AmongUs.GameOptions;
using JAM.Assets;
using JAM.Options.Roles.Neutral;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
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
            if (OptionGroupSingleton<MimicOptions>.Instance.BlacklistedRoles.GetRolesList().Any(role => role.Role == r.Role))
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

        if (role.IsImpostor()) return false; // Stupid Fix Right Now So Mimic At Least Can Be Played

        if (role.GetRoleAlignment() == RoleAlignment.NeutralOutlier) return false; // Removing Chef, Inquiz, etc.
        if (role.GetRoleAlignment() == RoleAlignment.CrewmatePower) return false; // No Crewmate Power
        if (role.GetRoleAlignment() == RoleAlignment.CrewmateSupport) return false; // No Crewmate Support

        return true; //Will Be Ok Here
    }
}
