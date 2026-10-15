using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using JAM.Modules.Components;
using JAM.Options.Roles.Neutral;
using JAM.Roles.Neutral;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Networking;
using MiraAPI.Roles;
using MiraAPI.Translation;
using MiraAPI.Utilities;
using Reactor.Networking.Attributes;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using static Il2CppSystem.Linq.Expressions.Interpreter.NullableMethodCallInstruction;
using Object = UnityEngine.Object;

namespace JAM.Options.OptionTypes;

// Make Saves for when you leave and come back
// Make Append Stuff Work

public class ModdedRolesOption(string title, List<RoleBehaviour> defaultList, bool includeInPreset = true) : ModdedOption<RoleTypes>(title, RoleTypes.Crewmate, includeInPreset)
{
    public ModdedRolesMenu? rolesMenu;
    public List<RoleBehaviour> rolesList = new List<RoleBehaviour>();

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

    protected override void OnValueChanged(RoleTypes newValue)
    {
        if (newValue == RoleTypes.Crewmate || newValue == RoleTypes.Impostor) return;

        RoleBehaviour role = DestroyableSingleton<RoleManager>.Instance.GetRole(newValue);

        if (!PlayerControl.LocalPlayer.IsHost()) ClickRoleHandle(role, false);

        bool addedRole = rolesList.Any(r => r.Role == newValue);

        StringOption? opt = base.OptionBehaviour as StringOption;
        if (opt != null)
        {
            opt.Value = (int)RoleTypes.Crewmate;
        }

        if(PlayerControl.LocalPlayer.IsHost())
        {
            RoleTypes[] roleTypes = OptionGroupSingleton<MimicOptions>.Instance.BlacklistedRoles.rolesList.Select(r => r.Role).ToArray();

            Rpc<SyncMimicBlacklistRpc>.Instance.Send(roleTypes);
        }

        ModdedOptionsManager.AddSettingsChangeMessage(DestroyableSingleton<HudManager>.Instance.Notifier, base.StringName, (rolesList.Any(r => r.Role == newValue) ? "Added " : "Removed ") + DestroyableSingleton<RoleManager>.Instance.GetRole((RoleTypes)newValue).GetRoleName(), this.Configuration.PopUpTextColor, this.Configuration.PopUpIconTmp, false);
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

        SetValue(role.Role, true);

        rolesMenu?.RefreshRoles();
    }

    public override float GetFloatData()
    {
        return 0;
    }

    public override NetData GetNetData()
    {
        return new NetData(base.Id, Encoding.Unicode.GetBytes(Convert.ToString(base.Value, NumberFormatInfo.InvariantInfo)));
    }

    public override void HandleNetData(byte[] data)
    {
        base.SetValue((RoleTypes)((object)Enum.Parse(typeof(RoleTypes), Encoding.Unicode.GetString(data))), true);
    }

    public override RoleTypes GetValueFromOptionBehaviour(OptionBehaviour optionBehaviour)
    {
        return (RoleTypes)((object)Enum.Parse(typeof(RoleTypes), optionBehaviour.GetInt().ToString(NumberFormatInfo.InvariantInfo)));
    }

    [HarmonyPatch(typeof(AbstractOptionGroup), "Options", MethodType.Getter)]
    public static class RemoveModRoleOptionFromOptions
    {
        public static void Postfix(ref List<IModdedOption> __result)
        {
            __result.RemoveAll(delegate (IModdedOption option)
            {
                if(option.GetType() == typeof(ModdedRolesOption))
                {
                    Info("Purging Role Option");
                    return true;
                }

                return false;
            });
        }
    }
}

//Useful To Have For Generalizing
/*
List<AbstractOptionGroup> optionGroups = AccessTools.Field(typeof(ModdedOptionsManager), "Groups").GetValue(null) as List<AbstractOptionGroup>;
AbstractOptionGroup abstractOptionGroup = optionGroups.FirstOrDefault((AbstractOptionGroup x) => x.OptionableType == classType);
*/