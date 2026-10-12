using UnityEngine;
using AmongUs.GameOptions;
using Il2CppInterop.Runtime.Attributes;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using MiraAPI.Modifiers;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using TownOfUs.Modules.MedSpirit;
using MiraAPI.Hud;
using MiraAPI.GameOptions;
using MiraAPI.Utilities.Assets;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using TownOfUs.Buttons.Impostor;
using TownOfUs;
using TownOfUs.Modules.Wiki;
using TownOfUs.Utilities;
using TownOfUs.Assets;
using TownOfUs.Roles;
using TownOfUs.Roles.Crewmate;
using TownOfUs.Extensions;
using JAM.Options.Roles.Impostor;
using JAM.Assets;
using JAM.Modifiers.Hidden;
using JAM.Buttons.Impostor;
using HarmonyLib;

namespace JAM.Roles.Impostor;

public sealed class ProjectorRole(IntPtr cppPtr) : ImpostorRole(cppPtr), ITownOfUsRole, IWikiDiscoverable, IDoomable
{
    public ProjectorKillButton? killButton;
    public bool isProjected = false;

    public RoleBehaviour CrewVariant => RoleManager.Instance.GetRole((RoleTypes)RoleId.Get<MediumRole>());
    public Color RoleColor => TownOfUsColors.Impostor;
    public string RoleName => "Projector";
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;
    public RoleAlignment RoleAlignment => RoleAlignment.ImpostorKilling;
    public DoomableType DoomHintType => DoomableType.Perception;

    public string RoleDescription => "Use your projection to become a ghost!";
    public string RoleMedDescriptionLocale => "Use your abiliy to hunt players as a ghost.";
    public string RoleLongDescription => "You can Project to temporirly turn into a ghost, killing players while projecte revives you at the bodies position.";
    public string GetAdvancedDescription()
    {
        return
            $"The Projector is a Impostor Killing that can use their Project ability to temporarily project as a ghost." +
            MiscUtils.AppendOptionsText(GetType());
    }

    public CustomRoleConfiguration Configuration => new(this)
    {
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(JamRoleIcons.Projector.LoadAsset(), "Projector", 1.45f),
        OptionsScreenshot = TouBanners.ImpostorRoleBanner,
        Icon = JamRoleIcons.Projector,
        UseVanillaKillButton = false
    };

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        if (Spirit == null) return;
        Spirit.DestroyImmediate();
        isProjected = false;
    }

    public override void OnMeetingStart()
    {
        RoleBehaviourStubs.OnMeetingStart(this);

        if (Spirit == null) return;
        Spirit.DestroyImmediate();
        isProjected = false;
    }

    public override void Initialize(PlayerControl player)
    {
        RoleBehaviourStubs.Initialize(this, player);
    }

    public MedSpiritObject? Spirit { get; set; }

    [MethodRpc((uint)TownOfUsRpc.Mediate)]
    public static void RpcMediate(PlayerControl player)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }

        if (!AmongUsClient.Instance.AmHost)
        {
            return;
        }

        var spirit = Instantiate(TouAssets.MediumSpirit.LoadAsset()).GetComponent<MedSpiritObject>();
        (player.Data.Role as ProjectorRole)?.Spirit = spirit;
        AmongUsClient.Instance.Spawn(spirit, player.OwnerId);
        (player.Data.Role as ProjectorRole)?.isProjected = true;
    }

    [MethodRpc((uint)JAMRpcCalls.MultiProject)]
    public static void RpcMultiMediate(PlayerControl player)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(player);
            return;
        }
        if (AmongUsClient.Instance.AmHost)
        {
            var spirit = Instantiate(TouAssets.MediumSpirit.LoadAsset()).GetComponent<MedSpiritObject>();
            AmongUsClient.Instance.Spawn(spirit, player.OwnerId);
        }
    }

    [MethodRpc((uint)TownOfUsRpc.RemoveMediumSpirit)]
    public static void RpcRemoveMediumSpirit(PlayerControl projector, MedSpiritObject spirit)
    {
        if (LobbyBehaviour.Instance)
        {
            MiscUtils.RunAnticheatWarning(projector);
            return;
        }

        spirit.StartCoroutine(spirit.CoDestroy().WrapToIl2Cpp());
        (projector.Data.Role as ProjectorRole)?.isProjected = false;
    }

    [HideFromIl2Cpp]
    public List<CustomButtonWikiDescription> Abilities
    {
        get
        {
            return
            [
                new($"Project", $"Project allows you to temporarily become a ghost leaving your body behind. You can kill while projected, but you will be revived at the bodies position.",
                    JamAssets.ProjectButton),
            ];
        }
    }
}
