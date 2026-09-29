using Il2CppInterop.Runtime.Attributes;
using JackOfAllMods.Assets;
using JackOfAllMods.Options.Roles.Crewmate;
using MiraAPI.GameOptions;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TownOfUs.Assets;
using TownOfUs.Extensions;
using TownOfUs.Modules;
using TownOfUs.Modules.Wiki;
using TownOfUs.Roles;
using TownOfUs.Utilities;
using UnityEngine;

namespace JackOfAllMods.Roles.Crewmate;

public sealed class GossipRole(IntPtr cppPtr) : CrewmateRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    static bool OnlyOneType = true;

        public DoomableType DoomHintType => DoomableType.Insight;
        public string LocaleKey => "Gossip";
        public string RoleName => "Gossip";
        public string RoleDescription => "Share some local lore!";
        public string RoleLongDescription => "Overhear players, then gossip about them in the meeting.";

        public string GetAdvancedDescription() { return RoleLongDescription + MiscUtils.AppendOptionsText(base.GetType()); }

        [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return new List<CustomButtonWikiDescription>
            {
				new("Overhear", "Select A Player To Share Info About In The Meeting", TouModifierIcons.Crewpostor),
            };
        }
    }
    
        public static void GenerateGossip(PlayerControl player, List<RoleBehaviour> randomRolesList)
    {
        string alertString = "Gossip Has Been Spread About " + player.Data.PlayerName + "! View Details In The Chat!";
        MiraAPI.Utilities.Helpers.CreateAndShowNotification(alertString, JackOfAllMods.Colors.Gossip, new Vector3(0f, 1f, -20f), null, TouModifierIcons.Crewpostor.LoadAsset());

        string gossipString = "";

        randomRolesList.Shuffle();

        foreach(string roleName in randomRolesList.Select(role => role.GetRoleName()))
        {
            gossipString += ", #" + roleName.Replace(" ", "-");
        }

        gossipString = gossipString.Substring(2);
        gossipString = player.Data.PlayerName + " is one of the following roles:\n" + gossipString;

        MiscUtils.AddFakeChat(player.Data, "Gossip:", gossipString, false, true);
    }
    
        public Color RoleColor => JackOfAllMods.Colors.Gossip;
        public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
        public RoleAlignment RoleAlignment => RoleAlignment.CrewmateInvestigative;

        public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(K2RoleIcons.Gossip.LoadAsset(), "JackOfAllMods.Roles.Crewmate.Gossip", 1.45f),
        IntroSound = TouAudio.DetectiveIntroSound,
        Icon = K2RoleIcons.Gossip
    };
}
