using K2AmongUs.Modifiers.Neutral;
using K2AmongUs.Roles.Neutral;
using MiraAPI.Events;
using MiraAPI.Keybinds;
using MiraAPI.Modifiers;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Networking.Attributes;
using Reactor.Utilities;
using TownOfUs.Assets;
using TownOfUs.Buttons;
using TownOfUs.Events.TouEvents;
using TownOfUs.Modifiers.Game.Crewmate;
using TownOfUs.Modules;
using TownOfUs.Modules.TimeLord;
using TownOfUs.Networking;
using TownOfUs.Roles.Impostor;
using TownOfUs.Utilities;
using TownOfUs.Utilities.Appearances;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace K2AmongUs.Buttons.Neutral;

public class ZombieReviveButton : TownOfUsButton
{

    public override string Name => "REVIVE";

    public override BaseKeybind Keybind => Keybinds.PrimaryAction;

    public override Color TextOutlineColor => K2AndNull.Colors.Zombie;

    public override float Cooldown => OptionGroupSingleton<ZombieOptions>().Instance.ZombieReviveCd;

    public override bool ZeroIsInfinite { get; set; } = true;

    public override LoadableAsset<Sprite> Sprite => TouCrewAssets.ReviveSprite;


    public override bool Enabled(RoleBehaviour? role)
    {
        return role is ZombieRole || role is ZombieLeaderRole;
    }


    public override bool CanUse()
    {
        return Helpers.GetNearestDeadBodies(PlayerControl.LocalPlayer.transform.position, ShipStatus.Instance.MaxLightRadius * 0.1f, Helpers.CreateFilter(Constants.NotShipMask)).Any(b => MiscUtils.PlayerById(b.ParentId).Data.Role is not ZombieRole);
    }

    public override bool CanClick()
    {
        return Helpers.GetNearestDeadBodies(PlayerControl.LocalPlayer.transform.position, ShipStatus.Instance.MaxLightRadius * 0.1f, Helpers.CreateFilter(Constants.NotShipMask)).Any(b => MiscUtils.PlayerById(b.ParentId).Data.Role is not ZombieRole);
    }


    protected override void OnClick()
    {
        List<DeadBody> bodiesInRange = Helpers.GetNearestDeadBodies(PlayerControl.LocalPlayer.transform.position, ShipStatus.Instance.MaxLightRadius * 0.1f, Helpers.CreateFilter(Constants.NotShipMask)).Where(b => !(MiscUtils.PlayerById(b.ParentId).GetRoleWhenAlive() is ZombieLeaderRole)).ToList();

        if (bodiesInRange.Count > 0)
        {
            SetZombieRole(MiscUtils.PlayerById(bodiesInRange[0].ParentId), bodiesInRange[0]);
        }
    }


    public static void SetZombieRole(PlayerControl player, DeadBody body)
    {
        if (player.HasModifier<ZombieRevealedModifier>()) return;

        foreach (BaseModifier modifier in player.GetModifiers<BaseModifier>().Where(m => m is not IVisualAppearance))
        {
            player.RpcRemoveModifier(modifier.UniqueId);
        }

        if (player.Data.Role is not ZombieRole)
            player.RpcFullRevive(false, body.TruePosition, RoleId.Get<ZombieRole>(), true);
        else
            player.RpcFullRevive(false, body.TruePosition, RoleId.Get<ZombieRole>(), false);
    }
}