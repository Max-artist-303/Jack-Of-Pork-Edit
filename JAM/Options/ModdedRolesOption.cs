using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using JAM.Modules.Components;
using JAM.Options.Roles.Neutral;
using JAM.Roles.Neutral;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JAM.Options.OptionTypes;

// Make Incoming Clients Accept The List Properly
// Make Saves Work Properly

public class ModdedRolesOption(string title, List<RoleBehaviour> defaultList, bool includeInPreset = true) : ModdedEnumOption(title, 0, typeof(RoleTypes))
{
    [HarmonyPatch(typeof(LobbyBehaviour), "Start")]
    public static class RequestRoleOptionsPatch
    {
        public static void Postfix()
        {
            if (PlayerControl.LocalPlayer.IsHost())
            {
                Coroutines.Start(OptionGroupSingleton<MimicOptions>.Instance.BlacklistedRoles.SetValues());
            }
            else
            {
                RequestSetValues(OptionGroupSingleton<MimicOptions>.Instance.BlacklistedRoles);
            }
        }
    }

    public ModdedRolesMenu? rolesMenu;
    List<RoleBehaviour> rolesList = new List<RoleBehaviour>();

    public override OptionBehaviour CreateOption(ToggleOption toggleOpt, NumberOption numberOpt, StringOption stringOpt, PlayerOption playerOpt, Transform container)
    {
        ToggleOption toggleOption = Object.Instantiate<ToggleOption>(toggleOpt, Vector3.zero, Quaternion.identity, container);
        toggleOption.name = base.ParentMod.OptionsTitleText + ".RoleOption." + base.StringName;
        toggleOption.SetUpFromData(base.Data, 20);
        toggleOption.Title = base.StringName;
        toggleOption.TitleText.text = base.Title.Translate();
        toggleOption.CheckMark.enabled = false;
        toggleOption.OnValueChanged = new Action<OptionBehaviour>(OnRoleButtonClicked);
        base.OptionBehaviour = toggleOption;

        return toggleOption;
    }

    void OnRoleButtonClicked(OptionBehaviour optionBehaviour)
    {
        Info("Opening Roles Menu");

        if (rolesMenu != null || !PlayerControl.LocalPlayer.IsHost()) return;

        rolesMenu = ModdedRolesMenu.Create(this);
        rolesMenu.Begin(IsRoleValid, ClickRoleHandle);

        (OptionBehaviour as ToggleOption)?.CheckMark.enabled = false;
    }

    protected override void OnValueChanged(int newValue)
    {
        if (newValue == (int)RoleTypes.Crewmate || newValue == (int)RoleTypes.Impostor) return;

        RoleBehaviour role = DestroyableSingleton<RoleManager>.Instance.GetRole((RoleTypes)newValue);

        if (newValue == -1 && !PlayerControl.LocalPlayer.IsHost())
        {
            rolesList.RemoveAll(r => true);
            StringOption? option = base.OptionBehaviour as StringOption;
            if (option != null)
            {
                option.Value = 0;
            }
            return;
        }
        else if (newValue == -1)
        {
            return;
        }
        else if (!PlayerControl.LocalPlayer.IsHost()) ClickRoleHandle(role, false);

        bool addedRole = rolesList.Any(r => (int)r.Role == newValue);

        ModdedOptionsManager.AddSettingsChangeMessage(DestroyableSingleton<HudManager>.Instance.Notifier, base.StringName, (addedRole ? "Added " : "Removed ") + role.GetRoleName(), this.Configuration.PopUpTextColor, this.Configuration.PopUpIconTmp, true);

        StringOption? opt = base.OptionBehaviour as StringOption;
        if (opt != null)
        {
            opt.Value = 0;
        }

    }

    private void ClickRoleHandle(RoleBehaviour role)
    {
        ClickRoleHandle(role, true);
    }

    bool IsRoleValid(RoleBehaviour role)
    {
        return MimicRole.IsValidMimicRole(role);
    }
    void ClickRoleHandle(RoleBehaviour role, bool isSetting)
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

        if (!isSetting) return;

        SetValue((int)role.Role, true);

        rolesMenu?.RefreshRoles();
    }

    public List<RoleBehaviour> GetRolesList()
    {
        return rolesList;
    }
    public void SetRolesList()
    {
        rolesList = rolesList;
    }

    public System.Collections.IEnumerator SetValues()
    {
        SetValue(-1, true);

        List<RoleBehaviour> skippedRoles = new List<RoleBehaviour>();

        foreach(RoleBehaviour role in rolesList)
        {
            yield return null;
            try
            {
                SetValue((int)role.Role, true);
            }
            catch
            {
                skippedRoles.Add(role);
            }
        }

        while(skippedRoles.Count > 0)
        {
            List<RoleBehaviour> tempRoleList = new List<RoleBehaviour>();
            foreach(RoleBehaviour role in skippedRoles)
            {
                yield return null;
                try
                {
                    SetValue((int)role.Role, true);
                }
                catch
                {
                    tempRoleList.Add(role);
                }
            }
            skippedRoles = tempRoleList;
        }

        rolesMenu = null;
    }

    [MethodRpc((uint)JAMRpcCalls.RequestRolesOption)]
    public static void RequestSetValues(ModdedRolesOption rolesOption)
    {
        if (PlayerControl.LocalPlayer.IsHost())
        {
            Coroutines.Start(rolesOption.SetValues());
        }
    }
}