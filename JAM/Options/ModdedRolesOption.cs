using AmongUs.GameOptions;
using JAM.Modules.Components;
using JAM.Roles.Neutral;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JAM.Options.OptionTypes;

// Add notif for when a role is added
// ENSURE THAT IT WORKS NON-LOCALLY!!!

public class ModdedRolesOption(string title, List<RoleBehaviour> rolesList, bool includeInPreset = true) : ModdedToggleOption(title, false, includeInPreset)
{
    public ModdedRolesMenu? rolesMenu;
    protected override void OnValueChanged(bool newValue)
    {
        (base.OptionBehaviour as ToggleOption)?.CheckMark.enabled = false;

        if (rolesMenu != null || !PlayerControl.LocalPlayer.IsHost()) return;

        rolesMenu = ModdedRolesMenu.Create(this);
        rolesMenu.Begin(IsRoleValid, ClickRoleHandle);
    }
    bool IsRoleValid(RoleBehaviour role)
    {
        return MimicRole.IsValidMimicRole(role);
    }
    void ClickRoleHandle(RoleBehaviour role)
    {
        bool addedRole = false;
        if (rolesList.Any(r => r.Role == role.Role))
        {
            rolesList.RemoveAll(r => r.Role == role.Role);
        }
        else
        {
            rolesList.Add(role);
            addedRole = true;
        }

        UpdateMimicBlacklist(this, rolesList.Select(r => (uint)r.Role).ToArray());

        ModdedOptionsManager.AddSettingsChangeMessage(DestroyableSingleton<HudManager>.Instance.Notifier, base.StringName, (addedRole ? "Added " : "Removed ") + role.GetRoleName(), this.Configuration.PopUpTextColor, this.Configuration.PopUpIconTmp, true);

        rolesMenu.RefreshRoles();
    }

    public List<RoleBehaviour> GetRolesList()
    {
        return rolesList;
    }
    public void SetRolesList(List<RoleBehaviour> newList)
    {
        rolesList = newList;
    }
}